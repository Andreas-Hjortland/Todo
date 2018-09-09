using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AcosTodo.Web;
using AcosTodo.Web.Entities;

namespace AcosTodo.Web.Services
{
    public class UserService
    {
        private const char Delimiter = '|';
        private const int OutputBytes = 20;
        private const int Iterations = 1000;
        private const int SaltSize = 32;

        private readonly TodoDbContext _context;
        public UserService(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(string username, string email, string about, string password)
        {
            var hash = HashPassword(password);
            var user = new User
            {
                Username = username,
                Email = email,
                About = about,
                PasswordHash = hash
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _context.Entry(user).State = EntityState.Detached;

            return user;
        }

        public Task<User> GetUser(int id)
        {
            return _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == id);
        }
        public Task<List<User>> GetUser(string username, string email)
        {
            return _context.Users
                .Where(u => u.Username == username || u.Email == email)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<User>> GetUsers()
        {
            return _context.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> GetValidUser(string username, string password)
        {
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Username == username || u.Email == username);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        private static string StringifyHash(Rfc2898DeriveBytes value, int length)
        {
            return $"{value.IterationCount}{Delimiter}{Convert.ToBase64String(value.Salt)}{Delimiter}{Convert.ToBase64String(value.GetBytes(length))}";
        }
        public static string HashPassword(string password)
        {
            Rfc2898DeriveBytes hashAlg = new Rfc2898DeriveBytes(
                password: password,
                saltSize: SaltSize,
                iterations: Iterations);

            return StringifyHash(hashAlg, OutputBytes);
        }
        private static bool VerifyPassword(string password, string hashCol)
        {
            var split = hashCol.Split(Delimiter);
            var iterations = int.Parse(split[0]);
            var salt = Convert.FromBase64String(split[1]);
            var hash = Convert.FromBase64String(split[2]);

            Rfc2898DeriveBytes hashAlg = new Rfc2898DeriveBytes(
                password: password,
                salt: salt,
                iterations: iterations);

            var result = StringifyHash(hashAlg, hash.Length);

            return result == hashCol;
        }
    }
}