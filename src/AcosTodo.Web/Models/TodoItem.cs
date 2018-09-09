using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AcosTodo.Web.Models
{
    public class TodoItem
    {
        public TodoItem() { }
        public TodoItem(Entities.TodoItem todoItem)
        {
            Id = todoItem.Id;
            Title = todoItem.Title;
            Description = todoItem.Description;
            Created = todoItem.Created;
            Completed = todoItem.Completed;
            Tags = todoItem.TagsMapping.Select(s => s.Tag.Name).ToList();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime Created { get; set; }

        public List<string> Tags { get; set; }
    }
}
