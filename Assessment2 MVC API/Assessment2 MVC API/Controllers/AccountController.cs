using Assessment2_MVC_API.Data;
using Assessment2_MVC_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Assessment2_MVC_API.Controllers
{
    // https://youtu.be/T0ZnrENlOfw?list=PL82C6-O4XrHde_urqhKJHH-HTUfTK6siO - carefully go through video

    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;  
        private RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) 
        { 
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Create Account
        public IActionResult Create()
        {
            return View();
        }
        
        // Create Role
        public IActionResult CreateRole() 
        { 
             return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAccount(User user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                {
                    ViewBag.Message = "User created successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = userRole.RoleName });
                if (result.Succeeded)
                {
                    ViewBag.Message = "Role created successfully.";
                }
                else
                {
                    foreach(IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }
    }
}
