using AcosTodo.Web.Entities;
using AcosTodo.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AcosTodo.Web.Tests
{
    public class TodoItemTest
    {
        public TodoItemTest() { }

        private static async Task<(TodoDbContext context, TodoService todoService, User user)> Init()
        {
            var context = DbContext.Get();
            var todoService = new TodoService(context);

            var userService = new UserService(context);
            var user = await userService.CreateUser("username", "email@example.com", null, "password");

            return (context, todoService, user);
        }

        [Fact]
        public async Task AddTodoItemIsPersisted()
        {
            var (context, todoService, user) = await Init();

            var count = await context.TodoItems.CountAsync();
            Assert.Equal(0, count);

            var tagCount = await context.Tags.CountAsync();
            Assert.Equal(0, tagCount);

            var tags = new List<string> { "tag1", "tag2" };
            var item = await todoService.AddTodoItem(new Models.TodoItem
            {
                Title = "title",
                Tags = tags,
            }, user.Id);

            count = await context.TodoItems.CountAsync();
            Assert.Equal(1, count);

            tagCount = await context.Tags.CountAsync();
            Assert.Equal(tags.Count, tagCount);

            Assert.All(item.TagsMapping, mapping =>
            {
                Assert.Contains(mapping.Tag.Name, tags);
            });

            var item2 = await todoService.GetTodoItem(item.Id);
            Assert.Equal("title", item2.Title);
        }

        [Fact]
        public async Task GetByOwnerGivesAllTodoItems()
        {
            var (context, todoService, user) = await Init();
            var item1 = await todoService.AddTodoItem(new Models.TodoItem
            {
                Title = "title",
                Tags = new List<string>(),
            }, user.Id);
            var item2 = await todoService.AddTodoItem(new Models.TodoItem
            {
                Title = "title2",
                Tags = new List<string>(),
            }, user.Id);

            var items = await todoService.GetTodoItemByOwner(user.Id);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Title == item1.Title);
            Assert.Contains(items, i => i.Title == item2.Title);
        }

        [Fact]
        public async Task ToggleCompletedSetsCompletedDate()
        {
            var (context, todoService, user) = await Init();
            var item1 = await todoService.AddTodoItem(new Models.TodoItem
            {
                Title = "title",
                Tags = new List<string>(),
            }, user.Id);

            Assert.Null(item1.Completed);
            item1 = await todoService.ToggleCompleted(item1.Id);
            Assert.NotNull(item1.Completed);

            item1 = await todoService.ToggleCompleted(item1.Id);
            Assert.Null(item1.Completed);
        }

        [Fact]
        public async Task RemoveItemIsPersisted()
        {
            var (context, todoService, user) = await Init();
            var item1 = await todoService.AddTodoItem(new Models.TodoItem
            {
                Title = "title",
                Tags = new List<string>(),
            }, user.Id);

            var result = await todoService.GetTodoItem(item1.Id);
            Assert.NotNull(result);
            await todoService.RemoveItem(item1.Id);

            result = await todoService.GetTodoItem(item1.Id);
            Assert.Null(result);
        }

        [Fact] 
        public async Task UpdateItemIsPersisted()
        {
            var (context, todoService, user) = await Init();
            var item = await todoService.AddTodoItem(new Models.TodoItem
            {
                Title = "title",
                Tags = new List<string> { "test", "baz" }
            }, user.Id);

            Assert.Equal("title", item.Title);

            var newTags = new List<string> { "test", "foo", "bar" };
            await todoService.UpdateTodoItem(new Models.TodoItem
            {
                Id = item.Id,
                Description = "Foobar",
                Title = "something else",
                Tags = newTags
            });

            var itemUpdated = await todoService.GetTodoItem(item.Id);
            Assert.Equal(item.Id, itemUpdated.Id);
            Assert.Equal("Foobar", itemUpdated.Description);
            Assert.Equal("something else", itemUpdated.Title);

            Assert.Equal(newTags.Count, itemUpdated.TagsMapping.Count);
            foreach(var tag in newTags)
            {
                Assert.Contains(tag, itemUpdated.TagsMapping.Select(s => s.Tag.Name));
            }

        }
    }
}
