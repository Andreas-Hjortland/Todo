using System.Collections.Generic;

namespace AcosTodo.Web.Models
{
    public class PaginatedTodoList
    {
        public IEnumerable<TodoItem> TodoItems { get; set; }
        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
