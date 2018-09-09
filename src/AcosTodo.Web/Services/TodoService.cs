using AcosTodo.Web.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcosTodo.Web.Services
{
    public class TodoService
    {
        private readonly TodoDbContext _context;

        public TodoService(TodoDbContext context)
        {
            _context = context;
        }

        private static IQueryable<TodoItem> AddIncludes(IQueryable<TodoItem> query)
        {
            return query
                .Include(t => t.TagsMapping)
                .ThenInclude(t => t.Tag);
        }

        private async Task UpdateTodoTags(TodoItem item, IEnumerable<string> tags)
        {
            var existingTags = await _context.Tags
                .Where(t => tags.Any(s => s == t))
                .ToDictionaryAsync(t => t.Name);

            var existingMappings = item.TagsMapping?.ToDictionary(m => m.Tag.Name) ?? new Dictionary<string, TodoItemTag>();

            item.TagsMapping = tags
                .Select(t =>
                {
                    TodoItemTag result;
                    if (existingMappings.TryGetValue(t, out result))
                    {
                        return result;
                    }
                    result = new TodoItemTag
                    {
                        TodoItem = item
                    };
                    if (existingTags.TryGetValue(t, out Tag tag))
                    {
                        result.Tag = tag;
                    }
                    else
                    {
                        result.Tag = new Tag { Name = t };
                    }
                    return result;
                })
                .ToList();

            foreach(var mapping in existingMappings)
            {
                if(item.TagsMapping.All(x => x != mapping.Value))
                {
                    _context.Remove(mapping.Value);
                }
            }
        }

        private void StopTracking(TodoItem item)
        {
            _context.Entry(item).State = EntityState.Detached;
            foreach(var mapping in item.TagsMapping)
            {
                _context.Entry(mapping).State = EntityState.Detached;
                _context.Entry(mapping.Tag).State = EntityState.Detached;
            }
        }

        public Task<TodoItem> GetTodoItem(int id) =>
            AddIncludes(_context.TodoItems)
                .AsNoTracking()
                .SingleOrDefaultAsync(t => t.Id == id);

        public Task<List<TodoItem>> GetTodoItemByOwner(int id) =>
            AddIncludes(_context.TodoItems)
                .Where(t => t.OwnerId == id)
                .AsNoTracking()
                .ToListAsync();

        public async Task<TodoItem> AddTodoItem(Models.TodoItem value, int userId)
        {
            var existingTags = await _context.Tags
                .Where(t => value.Tags.Contains(t))
                .ToDictionaryAsync(t => t.Name);

            var item = new TodoItem
            {
                Title = value.Title,
                Description = value.Description,
                OwnerId = userId,

                Completed = null,
                Created = DateTime.Now,
            };
            await UpdateTodoTags(item, value.Tags);
            _context.Add(item);

            await _context.SaveChangesAsync();
            StopTracking(item);

            return item;
        }
        
        public async Task<TodoItem> UpdateTodoItem(Models.TodoItem value)
        {
            var item = await GetTodoItem(value.Id);
            _context.Entry(item).State = EntityState.Unchanged;
            if(item == null)
            {
                throw new KeyNotFoundException($"Did not find todo item with id {value.Id}");
            }
            item.Title = value.Title;
            item.Description = value.Description;
            item.Created = value.Created;
            item.Completed = value.Completed;
            await UpdateTodoTags(item, value.Tags);

            await _context.SaveChangesAsync();
            StopTracking(item);

            return item;
        }

        public async Task<TodoItem> ToggleCompleted(int id)
        {
            var item = await GetTodoItem(id);
            if (item.Completed.HasValue)
            {
                item.Completed = null;
            }
            else
            {
                item.Completed = DateTime.Now;
            }
            _context.Update(item);
            await _context.SaveChangesAsync();
            StopTracking(item);
            return item;
        }

        public async Task<int> RemoveItem(int id)
        {
            var item = new TodoItem
            {
                Id = id
            };
            _context.Attach(item);
            _context.Remove(item);
            return await _context.SaveChangesAsync();
        }
    }
}
