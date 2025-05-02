using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EHRsystem.Data;
using EHRsystem.Models.Entities;

namespace EHRsystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // === GET: Book Appointment ===
        public IActionResult Book()
        {
            return View();
        }

        // === GET: Manage Appointments (Doctor View) ===
        public IActionResult Manage()
        {
            if (HttpContext.Session.GetString("UserRole") != "Doctor")
                return RedirectToAction("Login", "Account");

            var doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var appointments = _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .ToList();

            return View(appointments); // Send appointments to the view
        }

        // === POST: Book Appointment ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BookAppointment(Appointment appointment)
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            if (!ModelState.IsValid)
                return View("Book", appointment);

            appointment.PatientId = HttpContext.Session.GetInt32("UserId") ?? 0;
            appointment.Status = "Pending";

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", "Patient");
        }
    }
}
