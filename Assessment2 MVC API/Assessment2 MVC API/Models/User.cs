using System.ComponentModel.DataAnnotations;

namespace Assessment2_MVC_API.Models
{
    public class User
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }   
        [Required]
        public string Password { get; set; }

    }
}
