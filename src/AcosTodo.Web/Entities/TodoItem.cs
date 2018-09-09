using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcosTodo.Web.Entities
{
    public class TodoItem
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Completed { get; set; } = null;
        public DateTime Created { get; set; } = DateTime.Now;

        public List<TodoItemTag> TagsMapping { get; set; }

        [Required]
        public int OwnerId { get; set; }
        public User Owner { get; set; }
    }
}
