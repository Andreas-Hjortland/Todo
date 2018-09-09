using AcosTodo.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace AcosTodo.Web
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TodoItemTag> TodoItemTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Username);

            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Email);

            modelBuilder.Entity<TodoItemTag>()
                .HasAlternateKey(nameof(TodoItemTag.TodoItemId), nameof(TodoItemTag.TagId));

            modelBuilder.Entity<Tag>()
                .HasAlternateKey(t => t.Name);
        }
    }
}
