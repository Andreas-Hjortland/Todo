using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcosTodo.Web.Models;
using AcosTodo.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcosTodo.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;
        private readonly UserTokenService _userTokenService;
        public TodoController(TodoService todoService, UserTokenService userTokenService)
        {
            _todoService = todoService;
            _userTokenService = userTokenService;
        }

        // GET api/values
        [Authorize(Policy = "admin")]
        [HttpGet("byUser/{userid}")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> Get(int userId)
        {
            var items = await _todoService.GetTodoItemByOwner(userId);

            return items
                .Select(t => new TodoItem(t))
                .ToList();
        }

        [HttpGet()]
        public Task<ActionResult<IEnumerable<TodoItem>>> Get()
        {
            var userId = _userTokenService.GetUserId(User);
            return Get(userId.Value);
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> Post([FromBody] TodoItem value)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            var userId = _userTokenService.GetUserId(User);
            var result = await _todoService.AddTodoItem(value, userId.Value);
            return new TodoItem(result);
        }

        [HttpPost("toggleCompleted/{id}")]
        public async Task<TodoItem> ToggleCompleted(int id)
        {
            var result = await _todoService.ToggleCompleted(id);
            return new TodoItem(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItem>> Put(int id, [FromBody] TodoItem value)
        {
            if(!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            var userId = _userTokenService.GetUserId(User);
            value.Id = id;
            var result = await _todoService.UpdateTodoItem(value);

            return new TodoItem(result);
        }

        [HttpDelete("{id}")]
        public Task<int> Delete(int id)
        {
            // TODO authorize
            return _todoService.RemoveItem(id);
        }
    }
}
