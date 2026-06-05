using Assessment2_MVC_Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Assessment2_MVC_Web.Controllers
{
    public class OperationsController : Controller
    {
        private UserManager<ApplicationUser> userManager;

        public OperationsController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public ViewResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (!string.Equals(user.Password, user.ConfirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError(nameof(user.ConfirmPassword), "Passwords do not match.");
            }

            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.UserName,
                    Email = user.Email
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(appUser, "User");
                    TempData["SuccessMessage"] = "Account created successfully! Please log in.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
    }
}
