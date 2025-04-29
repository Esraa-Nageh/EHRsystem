using Microsoft.AspNetCore.Mvc;

namespace UnifiedEHRSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Welcome to Unified EHR System! Go to /Account/Register to start.");
        }
    }
}
