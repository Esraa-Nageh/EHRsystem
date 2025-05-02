using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EHRsystem.Data;
using EHRsystem.Models.Base;

namespace EHRsystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // === Admin Dashboard ===
        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return Unauthorized();
            return View();
        }

        // === Manage Users ===
        public IActionResult ManageUsers()
        {
            if (!IsAdmin()) return Unauthorized();
            var users = _context.Users.ToList();
            return View(users);
        }

        // === Create User (GET) ===
        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!IsAdmin()) return Unauthorized();
            return View();
        }

        // === Create User (POST) ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(User user, string Password)
        {
            if (!IsAdmin()) return Unauthorized();

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("", "Email already exists.");
                return View(user);
            }

            user.PasswordHash = HashPassword(Password);
            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("ManageUsers");
        }

        // === Edit User (GET) ===
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // === Edit User (POST) ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(User updatedUser)
        {
            if (!IsAdmin()) return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user == null) return NotFound();

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Role = updatedUser.Role;

            _context.SaveChanges();
            return RedirectToAction("ManageUsers");
        }

        // === Delete User (GET) ===
        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // === Delete User (POST) ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDeleteUser(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("ManageUsers");
        }

        // === Password Hashing Utility ===
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
