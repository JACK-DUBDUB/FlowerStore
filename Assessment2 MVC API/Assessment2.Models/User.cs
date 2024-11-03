using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment2.Models
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

        // Add a shopping cart public ?? <- not necessary
        //List<Product> ShoppingCart { get; set; } = new List<Product>();
    }
}
