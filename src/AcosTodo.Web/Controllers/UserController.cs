using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AcosTodo.Web;
using AcosTodo.Web.Models;
using AcosTodo.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AcosTodo.Web.Controllers
{
    [Authorize()]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly UserTokenService _userTokenService;
        public UserController(UserService userService, UserTokenService tokenService)
        {
            _userService = userService;
            _userTokenService = tokenService;
        }

        [Authorize(Policy = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            User.Claims.Any(s => s.Type == "admin" && s.Value == "true");
            var userEntities = await _userService.GetUsers();

            return userEntities.Select(u => new User
            {
                Id = u.Id,
                About = u.About,
                Email = u.Email,
                Username = u.Username,
            }).ToList();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> CreateUser([FromBody] User user) 
        {
            var existingUsers = await _userService.GetUser(user.Username, user.Email);
            foreach (var duplicate in existingUsers)
            {
                // TODO Possible information leak
                if (duplicate.Username == user.Username)
                {
                    ModelState.AddModelError(nameof(user.Username), $"It already exists a user with the name {user.Username}");
                }
                else
                {
                    ModelState.AddModelError(nameof(user.Email), $"It already exists a user with the email {user.Email}");
                }
            }
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _userService.CreateUser(user.Username, user.Email, user.About, user.Password);
            return await RequestToken(new TokenRequest
            {
                Username = user.Username,
                Password = user.Password
            });
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<ActionResult> RequestToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            var user = await _userService.GetValidUser(request.Username, request.Password);
            if(user == null) 
            {
                return new ForbidResult();
            }


            return new OkObjectResult(new {
                token = new JwtSecurityTokenHandler().WriteToken(_userTokenService.BuildToken(user))
            });
        }

        [HttpPost("token/renew")]
        public async Task<ActionResult> RenewToken() 
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (int.TryParse(claim?.Value, out int userid))
            {
                var user = await _userService.GetUser(userid);
                if (user != null)
                {
                    return new OkObjectResult(new {
                        token = new JwtSecurityTokenHandler().WriteToken(_userTokenService.BuildToken(user))
                    });
                }
            }
            return new BadRequestObjectResult("Unable to get user based on token. Try to request a new token");
        }
    }
}