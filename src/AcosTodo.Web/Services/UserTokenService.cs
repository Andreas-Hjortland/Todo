using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AcosTodo.Web.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AcosTodo.Web.Services {
    public class UserTokenServiceOptions
    {
        public string Issuer { get; set; } = "no.acos.todo";
        public string Audience { get; set; } = "no.acos.todo";
        public string SharedKey { get; set; }
        public int Timeout { get; set; } = 1800;

        public SecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SharedKey));
    }

    public class UserTokenService
    {
        private readonly UserService _userService;
        private readonly UserTokenServiceOptions _options;

        public UserTokenService(UserService userService, IOptions<UserTokenServiceOptions> options)
        {
            _userService = userService;
            _options = options.Value;
        }

        public int? GetUserId(ClaimsPrincipal principal)
        {
            var claim = principal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            if (int.TryParse(claim?.Value, out int id))
            {
                return id;
            }
            return null;
        }

        public async Task<User> GetUser(ClaimsPrincipal principal)
        {
            var id = GetUserId(principal);
            if (id.HasValue) {
                return await _userService.GetUser(id.Value);
            }
            return null;
        }

        public JwtSecurityToken BuildToken(User user)
        {
            var creds = new SigningCredentials(_options.SecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("admin", "true"),
            };
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddSeconds(_options.Timeout),
                signingCredentials: creds
            );
            return token;
        }
    }
}