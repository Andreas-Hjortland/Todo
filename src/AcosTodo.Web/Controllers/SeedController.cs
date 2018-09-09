using AcosTodo.Web;
using AcosTodo.Web.Entities;
using AcosTodo.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AcosTodo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly TodoDbContext _context;
        private readonly UserService _userService;

        public SeedController(TodoDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<int>> SeedData()
        {
            if(await _context.TodoItems.CountAsync() > 0)
            {
                return new BadRequestResult();
            }

            var user1 = await _userService.CreateUser("andreas", "andreas@hjortland.org", "This is a simple user", "pa55w0rd");
            var user2 = await _userService.CreateUser("test", "test@hjortland.org", "This is a test user", "foobar");
            var user3 = await _userService.CreateUser("foo", "foo@example.com", "Example user", "foo");

            var tag1 = new Tag
            {
                Name = "test"
            };
            var tag2 = new Tag
            {
                Name = "app"
            };
            var tag3 = new Tag
            {
                Name = "tag3"
            };
            var tag4 = new Tag
            {
                Name = "tag4"
            };

            var items = new TodoItem[]
            {
                new TodoItem
                {
                    Title = "Create a todo app",
                    Description = "Implement the api",
                    Completed = DateTime.Now.Subtract(TimeSpan.FromHours(1)),
                    Owner = user1,
                    TagsMapping = new List<TodoItemTag> {
                        new TodoItemTag { Tag = tag1 },
                        new TodoItemTag { Tag = tag2 },
                        new TodoItemTag { Tag = tag4 },
                    },
                },
                new TodoItem
                {
                    Title = "Take out the trash",
                    Description = "Remember the paper trash as well",
                    Completed = null,
                    Owner = user3,
                    TagsMapping = new List<TodoItemTag> {
                        new TodoItemTag { Tag = tag3 },
                        new TodoItemTag { Tag = tag4 },
                    },
                },
                new TodoItem
                {
                    Title = "Take out the trash",
                    Description = "Remember the paper trash as well",
                    Completed = null,
                    Owner = user2,
                    TagsMapping = new List<TodoItemTag> {
                        new TodoItemTag { Tag = tag2 },
                    },
                },
                new TodoItem
                {
                    Title = "Implement a frontend",
                    Description = "Implement the frontend of the app",
                    Completed = null,
                    Owner = user1,
                    TagsMapping = new List<TodoItemTag> {
                        new TodoItemTag { Tag = tag1 },
                        new TodoItemTag { Tag = tag3 },
                    },
                },
                new TodoItem
                {
                    Title = "Implement user auth",
                    Description = "OAuth and all the stuff",
                    Completed = null,
                    Owner = user1,
                    TagsMapping = new List<TodoItemTag> { },
                }
            };

            _context.AddRange(items);
            return await _context.SaveChangesAsync();
        }
    }
}
