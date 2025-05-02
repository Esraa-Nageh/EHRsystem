using Microsoft.AspNetCore.Mvc;

namespace UnifiedEHRSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Automatically redirect to the Register page
            return RedirectToAction("Register", "Account");
        }
    }
}
