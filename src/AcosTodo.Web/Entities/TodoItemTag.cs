using System;
using System.Collections.Generic;
using System.Text;

namespace AcosTodo.Web.Entities
{
    public class TodoItemTag
    {
        public int Id { get; set; }

        public TodoItem TodoItem { get; set; }
        public int TodoItemId { get; set; }

        public Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}
