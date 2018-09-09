using AcosTodo.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AcosTodo.Web.Tests
{
    public class UserServiceTest
    {
        public UserServiceTest()
        {
        }

        [Fact]
        public async Task AddUserShouldBePersisted()
        {
            var context = DbContext.Get();
            var userService = new UserService(context);

            var user = await userService.CreateUser("user1", "email@example.com", null, "password");
            Assert.True(context.Users.Any(s => s.Username == "user1"));

            var user2 = await userService.GetUser(user.Id);
            Assert.Equal(user.About, user2.About);
            Assert.Equal(user.Username, user2.Username);
            Assert.Equal(user.Email, user2.Email);
            Assert.Equal(user.PasswordHash, user2.PasswordHash);

            Assert.Equal(1, context.Users.Count());
        }

        [Fact]
        public async Task UserServiceShouldRetrieveUsersWithUsernameAndPassword()
        {
            var context = DbContext.Get();
            var userService = new UserService(context);

            var user = await userService.CreateUser("user2", "email2@example.com", null, "password");

            var user2 = await userService.GetValidUser("user2", "password");
            Assert.NotNull(user2);
            Assert.Equal(user.About, user2.About);
            Assert.Equal(user.Username, user2.Username);
            Assert.Equal(user.Email, user2.Email);
            Assert.Equal(user.PasswordHash, user2.PasswordHash);

            var user3 = await userService.GetValidUser("user3", "password");
            Assert.Null(user3);

            var user4 = await userService.GetValidUser("user2", "password2");
            Assert.Null(user4);
        }
    }
}
