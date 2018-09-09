using System.ComponentModel.DataAnnotations;

namespace AcosTodo.Web.Models
{
    public class TokenRequest
    {
        [Required]
        [MinLength(1)]
        public string Username { get; set; }

        [Required]
        [MinLength(1)]
        public string Password { get; set; }
    }
}