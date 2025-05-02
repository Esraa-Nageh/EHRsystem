using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using EHRsystem.Data;
using EHRsystem.Models.Base;
using EHRsystem.Models.Entities;

namespace EHRsystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // === Dashboard ===
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        // === Edit Profile ===
        [HttpGet]
        public IActionResult EditProfile()
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            int? id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User updatedUser, string? NewPassword)
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user == null)
                return NotFound();

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;

            if (!string.IsNullOrEmpty(NewPassword))
                user.PasswordHash = HashPassword(NewPassword);

            _context.SaveChanges();
            HttpContext.Session.SetString("UserName", user.Name);

            return RedirectToAction("Dashboard");
        }

        // === Book Appointment ===
        [HttpGet]
        public IActionResult BookAppointment()
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            var doctors = _context.Users
                .OfType<Doctor>()  // هنا السر
                .Select(d => new
                {
                    d.Id,
                    Name = d.Name,
                    d.Specialty,
                    d.Location
                })
                .ToList();

            ViewBag.Doctors = doctors;
            return View(new Appointment());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BookAppointment(Appointment appointment)
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                var doctors = _context.Users
       .OfType<Doctor>()  // هنا السر
       .Select(d => new
       {
           d.Id,
           Name = d.Name,
           d.Specialty,
           d.Location
       })
       .ToList();

                ViewBag.Doctors = doctors;
                return View(appointment);
            }

            appointment.PatientId = HttpContext.Session.GetInt32("UserId") ?? 0;
            appointment.Status = "Pending";
            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        // === Medical Records ===
        public IActionResult MedicalRecords()
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            int patientId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var files = _context.MedicalFiles
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.UploadedAt)
                .ToList();

            return View(files);
        }

        [HttpGet]
        public IActionResult EditMedicalFile(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            var file = _context.MedicalFiles.FirstOrDefault(f => f.Id == id);

            if (file == null || file.PatientId != HttpContext.Session.GetInt32("UserId"))
                return Unauthorized();

            return View(file);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMedicalFile(MedicalFile updated)
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            var file = _context.MedicalFiles.FirstOrDefault(f => f.Id == updated.Id);

            if (file == null || file.PatientId != HttpContext.Session.GetInt32("UserId"))
                return Unauthorized();

            file.FileType = updated.FileType;
            file.Description = updated.Description;

            _context.SaveChanges();

            return RedirectToAction("MedicalRecords");
        }

        [HttpGet]
        public IActionResult DeleteMedicalFile(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            var file = _context.MedicalFiles.FirstOrDefault(f => f.Id == id);

            if (file == null || file.PatientId != HttpContext.Session.GetInt32("UserId"))
                return Unauthorized();

            return View(file);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDeleteMedicalFile(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            var file = _context.MedicalFiles.FirstOrDefault(f => f.Id == id);

            if (file == null || file.PatientId != HttpContext.Session.GetInt32("UserId"))
                return Unauthorized();

            _context.MedicalFiles.Remove(file);
            _context.SaveChanges();

            return RedirectToAction("MedicalRecords");
        }

        // === Upload Medical File ===
        [HttpGet]
        public IActionResult UploadMedicalFile()
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMedicalFile(IFormFile file, string FileType, string Description)
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient")
                return Unauthorized();

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please select a valid file.");
                return View();
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var medicalFile = new MedicalFile
            {
                PatientId = HttpContext.Session.GetInt32("UserId") ?? 0,
                DoctorId = 0, // will be updated by doctor later
                FileType = FileType,
                Description = Description,
                FilePath = "/uploads/" + fileName,
                UploadedAt = DateTime.Now
            };

            _context.MedicalFiles.Add(medicalFile);
            _context.SaveChanges();

            return RedirectToAction("MedicalRecords");
        }

        // === Password Hashing Helper ===
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
