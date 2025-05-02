using Microsoft.AspNetCore.Mvc;

namespace EHRsystem.Controllers
{
    public class PrescriptionController : Controller
    {
        [HttpGet]
        public IActionResult AddPrescription()
        {
            if (HttpContext.Session.GetString("UserRole") != "Doctor")
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult AddPrescription(int patientId, string medication, string dosage)
        {
            // TODO: Save to database (mock for now)
            ViewBag.Message = "Prescription added successfully.";
            return View();
        }
    }
}
