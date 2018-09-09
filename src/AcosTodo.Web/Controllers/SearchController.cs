using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcosTodo.Web;
using AcosTodo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcosTodo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly TodoDbContext _context;
        private const int MaxResults = 50;
        public TagsController(TodoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            return await _context.Tags
                .Select(t => t.Name)
                .ToListAsync();
        }

        [HttpGet("search/{term}")]
        public async Task<ActionResult<IEnumerable<string>>> SearchTags(string term)
        {
            return await _context.Tags
                .Where(c => c.Name.Contains(term))
                .Select(t => t.Name)
                .Take(MaxResults)
                .ToListAsync();
        }
    }
}