using System.Collections.Generic;

namespace AcosTodo.Web.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string About { get; set; }
        public string PasswordHash { get; set; }

        // TODO Claims

        public List<TodoItem> TodoItems { get; set; }
    }
}
