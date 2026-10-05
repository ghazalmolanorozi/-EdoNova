using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LMS.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();
            return View("CourseIndex",courses);
        }

        public IActionResult Create()
        {
            ViewBag.Teachers = _context.Users
                .Where(x => x.Role == "Teacher")
                .ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }


            ViewBag.Teachers = _context.Users
                .Where(x => x.Role == "Teacher")
                .ToList();


            return View(course);
        }

        public IActionResult Edit(int id)
        {
            var course = _context.Courses.Find(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Update(course);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.Find(id);

            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}