using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult StudentIndex()
        {
            int? studentId = HttpContext.Session.GetInt32("UserID");

            if (studentId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var enrollments = _context.Enrollments
                .Where(x => x.StudentID == studentId.Value)
                .ToList();

            var courseIds = enrollments
                .Select(x => x.CourseID)
                .ToList();

            var courses = _context.Courses
                .Where(x => courseIds.Contains(x.CourseID))
                .ToList();

            int coursesCount = courses.Count;
            int lessonsCount = _context.Lessons
                .Count(x => courseIds.Contains(x.CourseID));

            int materialsCount = _context.CourseMaterials
                .Count(x => courseIds.Contains(x.CourseID));

            int teachersCount = courses
                .Select(x => x.TeacherID)
                .Distinct()
                .Count();


            int enrollment = enrollments.Count;

            ViewBag.CoursesCount = coursesCount;
            ViewBag.LessonsCount = lessonsCount;
            ViewBag.MaterialsCount = materialsCount;
            ViewBag.TeachersCount = teachersCount;
            ViewBag.EnrollmentCount = enrollments.Count;

            ViewBag.Courses = courses;

            return View();
        }
        public IActionResult Courses()
        {
            int? studentId = HttpContext.Session.GetInt32("UserID");

            if (studentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var courses = _context.Courses
                .ToList();

            var enrolledCourseIds = _context.Enrollments
                .Where(x => x.StudentID == studentId.Value)
                .Select(x => x.CourseID)
                .ToList();

            ViewBag.EnrolledCourseIds = enrolledCourseIds;

            return View(courses);
        }

        public IActionResult Enroll(int id)
        {
            int? studentId = HttpContext.Session.GetInt32("UserID");

            if (studentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var exists = _context.Enrollments
                .Any(x => x.StudentID == studentId.Value && x.CourseID == id);

            if (exists)
            {
                TempData["Message"] = "شما قبلاً در این دوره ثبت‌نام کرده‌اید.";
                return RedirectToAction("Courses");
            }

            var course = _context.Courses
                .FirstOrDefault(x => x.CourseID == id);

            if (course == null)
            {
                return NotFound();
            }

            var enrollment = new Enrollment
            {
                StudentID = studentId.Value,
                CourseID = id
            };

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();

            TempData["Message"] = "ثبت‌نام شما با موفقیت انجام شد.";

            return RedirectToAction("Courses");
        }

        public IActionResult MyCourses()
        {
            int? studentId = HttpContext.Session.GetInt32("UserID");

            if (studentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var courses = _context.Enrollments
                .Where(x => x.StudentID == studentId.Value)
                .Join(
                    _context.Courses,
                    e => e.CourseID,
                    c => c.CourseID,
                    (e, c) => c
                )
                .ToList();

            return View(courses);
        }

        public IActionResult CourseDetails(int id)
        {
            int? studentId = HttpContext.Session.GetInt32("UserID");

            if (studentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var enrolled = _context.Enrollments
                .Any(x =>
                    x.StudentID == studentId.Value &&
                    x.CourseID == id);

            if (!enrolled)
            {
                return RedirectToAction("Courses");
            }

            var course = _context.Courses
                .FirstOrDefault(x => x.CourseID == id);

            if (course == null)
            {
                return NotFound();
            }

            var lessons = _context.Lessons
                .Where(x => x.CourseID == id)
                .OrderBy(x => x.LessonID)
                .ToList();

            var materials = _context.CourseMaterials
                .Where(x => x.CourseID == id)
                .OrderBy(x => x.CourseMaterialID)
                .ToList();

            ViewBag.Lessons = lessons;
            ViewBag.Materials = materials;

            return View(course);
        }
    }

}