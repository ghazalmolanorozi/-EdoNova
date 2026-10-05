using LMS.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using LMS.Models;

namespace LMS.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminIndex()
        {
            ViewBag.UsersCount = _context.Users.Count();

            ViewBag.StudentsCount = _context.Users
                .Count(x => x.Role == "Student");

            ViewBag.TeachersCount = _context.Users
                .Count(x => x.Role == "Teacher");

            ViewBag.AdminCount = _context.Users
                .Count(x => x.Role == "Admin");

            ViewBag.CoursesCount = _context.Courses.Count();

            ViewBag.LatestUsers = _context.Users
                .OrderByDescending(x => x.UserID)
                .Take(5)
                .ToList();

            return View();
        }
        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Users");
            }

            return View(user);
        }
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                _context.SaveChanges();

                return RedirectToAction("Users");
            }

            return View(user);
        }
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("Users");
        }
        public IActionResult Reports()
        {
            ViewBag.UsersCount = _context.Users.Count();

            ViewBag.StudentsCount = _context.Users
                .Count(x => x.Role == "Student");

            ViewBag.TeachersCount = _context.Users
                .Count(x => x.Role == "Teacher");

            ViewBag.CoursesCount = _context.Courses.Count();

            return View();
        }
    }
}