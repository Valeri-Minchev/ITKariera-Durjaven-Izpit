using Microsoft.AspNetCore.Mvc;
using CampRating.Data;
using Microsoft.EntityFrameworkCore;

namespace CampRating.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewBag.UserCount = _context.Users.Count();
            ViewBag.CampingSiteCount = _context.CampingSites.Count();
            ViewBag.ReviewCount = _context.Reviews.Count();
            return View();
        }

        public IActionResult ManageUsers()
        {
            var users = _context.Users
                .Include(u => u.Role)
                .Select(u => new CampRating.Models.User
                {
                    Id = u.Id,
                    Username = u.Username ?? "",
                    Password = u.Password ?? "",
                    FirstName = u.FirstName ?? "",
                    LastName = u.LastName ?? "",
                    Role = new CampRating.Models.Role
                    {
                        Id = u.Role.Id,
                        Name = u.Role.Name ?? ""
                    }
                })
                .Where(u => u.Role.Name != "Admin")
                .ToList();

            return View(users);
        }
        // Показва формата за редакция на потребител
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Include(u => u.Role)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(int id, CampRating.Models.User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            user.Username = updatedUser.Username;
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;

            _context.SaveChanges();

            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("ManageUsers");
        }

        public IActionResult ManageCampings()
        {
            var campings = _context.CampingSites
                .Select(c => new CampRating.Models.CampingSite
                {
                    Id = c.Id,
                    Name = c.Name ?? "",
                    Description = c.Description ?? "",
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    PhotoPath = c.PhotoPath ?? ""
                })
                .ToList();

            return View(campings);
        }

        public IActionResult ManageReviews()
        {
            var reviews = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.CampingSite)
                .Select(r => new CampRating.Models.Review
                {
                    Id = r.Id,
                    Content = r.Content ?? "",
                    User = new CampRating.Models.User
                    {
                        Id = r.User.Id,
                        Username = r.User.Username ?? "",
                        FirstName = r.User.FirstName ?? "",
                        LastName = r.User.LastName ?? ""
                    },
                    CampingSite = new CampRating.Models.CampingSite
                    {
                        Id = r.CampingSite.Id,
                        Name = r.CampingSite.Name ?? "",
                        Description = r.CampingSite.Description ?? "",
                        Latitude = r.CampingSite.Latitude,
                        Longitude = r.CampingSite.Longitude,
                        PhotoPath = r.CampingSite.PhotoPath ?? ""
                    }
                })
                .ToList();

            return View(reviews);
        }
        [HttpPost]
        public IActionResult DeleteReview(int id)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            _context.SaveChanges();

            return RedirectToAction("ManageReviews");
        }

        [HttpGet]
        public IActionResult EditCamping(int id)
        {
            var camping = _context.CampingSites
                .Where(c => c.Id == id)
                .Select(c => new CampRating.Models.CampingSite
                {
                    Id = c.Id,
                    Name = c.Name ?? "",
                    Description = c.Description ?? "",
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    PhotoPath = c.PhotoPath ?? ""
                })
                .FirstOrDefault();

            if (camping == null)
            {
                return NotFound();
            }

            return View(camping);
        }

        // ➡️ Записва нов или обновява съществуващ Camping Site
        [HttpPost]
        public IActionResult EditCamping(int id, CampRating.Models.CampingSite updatedCamping, IFormFile photo)
        {
            CampRating.Models.CampingSite camping;

            if (id == 0)
            {
                // Създаваме нов запис
                camping = new CampRating.Models.CampingSite();
                _context.CampingSites.Add(camping);
            }
            else
            {
                // Търсим за редакция
                camping = _context.CampingSites.FirstOrDefault(c => c.Id == id);
                if (camping == null)
                {
                    return NotFound();
                }
            }

            // Попълваме данните
            camping.Name = updatedCamping.Name;
            camping.Description = updatedCamping.Description;
            camping.Latitude = updatedCamping.Latitude;
            camping.Longitude = updatedCamping.Longitude;

            // Ако е качена нова снимка
            if (photo != null && photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                camping.PhotoPath = "/uploads/" + fileName;
            }

            _context.SaveChanges();

            return RedirectToAction("ManageCampings");
        }
        [HttpGet]
        public IActionResult AddCamping()
        {
            var newCamping = new CampRating.Models.CampingSite();
            return View("EditCamping", newCamping); // Използва EditCamping.cshtml
        }
        [HttpPost]
        public IActionResult DeleteCamping(int id)
        {
            var camping = _context.CampingSites.FirstOrDefault(c => c.Id == id);

            if (camping == null)
            {
                return NotFound();
            }

            _context.CampingSites.Remove(camping);
            _context.SaveChanges();

            return RedirectToAction("ManageCampings");
        }
    }
}
