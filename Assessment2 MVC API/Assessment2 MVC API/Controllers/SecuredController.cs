using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment2_MVC_API.Controllers
{
    public class SecuredController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
