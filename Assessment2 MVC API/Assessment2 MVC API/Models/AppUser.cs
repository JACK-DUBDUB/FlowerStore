using Microsoft.AspNet.Identity.EntityFramework;

namespace Assessment2_MVC_API.Models
{
    public class AppUser : IdentityUser
    {
        public  ICollection<Product> Products { get; set; }
    }
}
