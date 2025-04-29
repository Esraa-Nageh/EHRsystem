using Microsoft.AspNetCore.Mvc;
using EHRsystem.Data;
using EHRsystem.Models.Base;
using EHRsystem.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EHRsystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ====== Register ======

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email already registered.");
                    return View(model);
                }

                var hashedPassword = HashPassword(model.Password);

                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    Role = model.Role
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        // ====== Login ======

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                if (user != null && VerifyPassword(model.Password, user.PasswordHash))
                {
                    // Save user info in session
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.Name);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // Redirect based on Role
                    if (user.Role == "Doctor")
                        return RedirectToAction("Dashboard", "Doctor");
                    else if (user.Role == "Patient")
                        return RedirectToAction("Dashboard", "Patient");
                    else
                        return RedirectToAction("Index", "Home"); // Default
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        // ====== Logout ======

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // ====== Helper methods ======

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var hashOfInput = HashPassword(enteredPassword);
            return hashOfInput == storedHash;
        }
    }
}
