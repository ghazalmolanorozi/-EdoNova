using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace LMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(string UserName, string Password, string Role)
        {
            var user = _context.Users.FirstOrDefault(x =>
                x.UserName == UserName &&
                x.Password == Password &&
                x.Role == Role);

            if (user == null)
            {
                ViewBag.Error = "نام کاربری، رمز عبور یا نقش انتخابی اشتباه است.";
                return View();
            }

            switch (user.Role)
            {
                case "Admin":
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("Role", user.Role);
                    return RedirectToAction("AdminIndex", "Admin");

                case "Teacher":
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("Role", user.Role);
                    return RedirectToAction("TeacherIndex", "Teacher");

                case "Student":
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("Role", user.Role);
                    return RedirectToAction("StudentIndex", "Student");

                default:
                    ViewBag.Error = "نقش کاربر معتبر نیست.";
                    return View();
            }
        }
    }
}