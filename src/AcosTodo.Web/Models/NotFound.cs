using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcosTodo.Web.Models
{
    public class NotFound<TKey>
    {
        public TKey Key { get; set; }
        public string EntityName { get; set; }
    }
}
