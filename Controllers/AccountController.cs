using Microsoft.AspNetCore.Mvc;
using CampRating.Data;
using CampRating.Models;
using Microsoft.EntityFrameworkCore;

namespace CampRating.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role.Name);

                if (user.Role.Name == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("List", "Camping");
                }
            }
            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string firstName, string lastName)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Username already exists.";
                return View();
            }

            var userRole = _context.Roles.FirstOrDefault(r => r.Name == "User");

            var newUser = new User
            {
                Username = username,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                RoleId = userRole.Id
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
