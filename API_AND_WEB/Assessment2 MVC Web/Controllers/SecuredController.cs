using Assessment2_MVC_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Assessment2_MVC_Web.Controllers
{
    [Authorize]
    public class SecuredController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SecuredController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = new List<string>();

            DateTime? createdOn = null;

            if (user != null)
            {
                roles = (await _userManager.GetRolesAsync(user)).ToList();

                // MongoIdentityUser commonly exposes CreatedOn (set at registration)
                var createdProp = user.GetType().GetProperty("CreatedOn")
                               ?? user.GetType().GetProperty("CreatedAt")
                               ?? user.GetType().GetProperty("CreateDate");

                if (createdProp != null)
                {
                    createdOn = createdProp.GetValue(user) as DateTime?;
                }
            }

            ViewBag.Roles = roles;
            ViewBag.CreatedOn = createdOn;

            return View();
        }
    }
}
