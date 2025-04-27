using Microsoft.AspNetCore.Mvc;
using CampRating.Data;
using CampRating.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace CampRating.Controllers
{
    public class CampingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CampingController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult List(string search)
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
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                campings = campings.Where(c => c.Name.Contains(search));
            }

            return View(campings.ToList());
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CampingSite campingSite, IFormFile Photo)
        {
            if (Photo != null && Photo.Length > 0)
            {
                if (Photo.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("Photo", "File size must be under 2MB.");
                    return View();
                }

                var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var filePath = Path.Combine(uploads, Photo.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Photo.CopyTo(stream);
                }
                campingSite.PhotoPath = "/uploads/" + Photo.FileName;
            }

            _context.CampingSites.Add(campingSite);
            _context.SaveChanges();
            return RedirectToAction("ManageCampings", "Admin");
        }

        public IActionResult Edit(int id)
        {
            var camping = _context.CampingSites.Find(id);
            return View(camping);
        }

        [HttpPost]
        public IActionResult Edit(CampingSite campingSite, IFormFile Photo)
        {
            if (Photo != null && Photo.Length > 0)
            {
                if (Photo.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("Photo", "File size must be under 2MB.");
                    return View(campingSite);
                }

                var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var filePath = Path.Combine(uploads, Photo.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Photo.CopyTo(stream);
                }
                campingSite.PhotoPath = "/uploads/" + Photo.FileName;
            }

            _context.CampingSites.Update(campingSite);
            _context.SaveChanges();
            return RedirectToAction("ManageCampings", "Admin");
        }

        public IActionResult Delete(int id)
        {
            var camping = _context.CampingSites.Find(id);
            _context.CampingSites.Remove(camping);
            _context.SaveChanges();
            return RedirectToAction("ManageCampings", "Admin");
        }
    }
}
