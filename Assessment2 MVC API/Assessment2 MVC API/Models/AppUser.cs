using Microsoft.AspNetCore.Identity;

namespace Assessment2_MVC_API.Models
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

        public ICollection<Product> Products { get; set; }
        //public
    }
}
