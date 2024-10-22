using Assessment2_MVC_API.Data;
using Assessment2_MVC_API.Models;
using Assessment2_MVC_API.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Assessment2_MVC_API.Controllers
{
    // https://youtu.be/T0ZnrENlOfw?list=PL82C6-O4XrHde_urqhKJHH-HTUfTK6siO

    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly MongoDbService _contex;

        // dependency injector
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, MongoDbService contex)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _contex=contex;
        }   


        public IActionResult Login()
        {
            var reponse = new LoginViewModel();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if(!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

            if(user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password); // returns if the password is true or not
                if (passwordCheck)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, loginViewModel.Password, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", ""); // https://youtu.be/T0ZnrENlOfw?list=PL82C6-O4XrHde_urqhKJHH-HTUfTK6siO&t=945 explained here
                    }
                }

                // Incorrect Password
                TempData["Error"] = "Wrong credentials. Please try again.";
                return View(loginViewModel);
            }
            // User not found
            TempData["Error"] = "Wrong credentials. Please try again.";
            return View(loginViewModel);
        }

    }
}
