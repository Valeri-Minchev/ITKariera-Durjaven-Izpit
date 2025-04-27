using Microsoft.AspNetCore.Mvc;
using CampRating.Data;
using CampRating.Models;
using Microsoft.EntityFrameworkCore;

namespace CampRating.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult MyReviews()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var reviews = _context.Reviews
                .Where(r => r.UserId == userId)
                .Include(r => r.CampingSite)
                .Select(r => new CampRating.Models.Review
                {
                    Id = r.Id,
                    Content = r.Content ?? "",
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


        public IActionResult Create(int campingSiteId)
        {
            ViewBag.CampingSiteId = campingSiteId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Review review)
        {
            review.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            review.CreatedAt = DateTime.Now;
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return RedirectToAction("MyReviews");
        }

        public IActionResult Edit(int id)
        {
            var review = _context.Reviews.Find(id);
            return View(review);
        }

        [HttpPost]
        public IActionResult Edit(Review review)
        {
            review.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            review.CreatedAt = DateTime.Now;
            _context.Reviews.Update(review);
            _context.SaveChanges();
            return RedirectToAction("MyReviews");
        }

        public IActionResult Delete(int id)
        {
            var review = _context.Reviews.Find(id);
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return RedirectToAction("MyReviews");
        }
    }
}
