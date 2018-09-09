using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AcosTodo.Web.Tests
{
    public static class DbContext
    {
        public static TodoDbContext Get()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TodoDbContext>();
            optionsBuilder.UseInMemoryDatabase("test", new InMemoryDatabaseRoot());

            return new TodoDbContext(optionsBuilder.Options);
        }
    }
}
