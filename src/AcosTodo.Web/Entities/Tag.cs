using System.Collections.Generic;

namespace AcosTodo.Web.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<TodoItemTag> TodoItems { get; set; }

        public static implicit operator string(Tag value)
        {
            return value.Name;
        }
        public static implicit operator Tag(string value)
        {
            return new Tag
            {
                Name = value
            };
        }
    }
}
