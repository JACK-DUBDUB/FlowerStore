using Microsoft.AspNetCore.Mvc;

namespace Assessment2_MVC_Web.Controllers
{
    public class CartController : Controller
    {
        // Cart is primarily client-side (localStorage) for this demo store.
        // The Index page is a full rich cart experience with live editing + checkout simulation.
        public IActionResult Index()
        {
            ViewData["Title"] = "Your Cart";
            return View();
        }
    }
}
