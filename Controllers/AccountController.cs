using Microsoft.AspNetCore.Mvc;
using BusinessManagementSystem.Data;
using BusinessManagementSystem.Models;

namespace BusinessManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= LOGIN =================
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Email == email 
                                  && x.Password == password 
                                  && x.IsActive);

            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("UserName", user.Username);

                if (user.EmployeeId != null)
                {
                    HttpContext.Session.SetString("EmployeeId", user.EmployeeId.ToString());
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        // ================= SIGNUP =================
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(x => x.Email == model.Email))
                {
                    ViewBag.Error = "Email already exists";
                    return View(model);
                }

                model.CreatedDate = DateTime.Now;
                model.IsActive = true;
                model.RoleId = 3; // Default to User role

                _context.Users.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // ================= LOGOUT =================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
