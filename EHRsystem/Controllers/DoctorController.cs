using Microsoft.AspNetCore.Mvc;

namespace EHRsystem.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Dashboard()
        {
            // Session check (optional)
            if (HttpContext.Session.GetString("UserRole") != "Doctor")
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}
