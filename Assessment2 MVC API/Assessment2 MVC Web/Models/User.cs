using System.ComponentModel.DataAnnotations;

namespace Assessment2_MVC_Web.Models
{
    public class User
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
