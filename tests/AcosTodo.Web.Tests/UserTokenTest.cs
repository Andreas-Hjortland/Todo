using AcosTodo.Web.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Xunit;

namespace AcosTodo.Web.Tests
{
    public class UserTokenTest
    {
        public UserTokenTest()
        {

        }

        [Fact]
        public void UserTokenServiceCanExtractUserId()
        {
            var context = DbContext.Get();
            var userService = new UserService(context);
            var userTokenService = new UserTokenService(
                userService: userService, 
                options: Options.Create(new UserTokenServiceOptions()));

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            var id = userTokenService.GetUserId(principal);
            Assert.Equal(1, id);

            principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[0]));
            id = userTokenService.GetUserId(principal);
            Assert.Null(id);
        }
    }
}
