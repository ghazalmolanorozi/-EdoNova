using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace LMS.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // Teacher Dashboard
        // =========================
        public IActionResult TeacherIndex()
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var courses = _context.Courses
                .Where(x => x.TeacherID == teacherId.Value)
                .ToList();

            var courseIds = courses
                .Select(x => x.CourseID)
                .ToList();

            int coursesCount = courses.Count;

            int studentsCount = _context.Enrollments
                .Where(x => courseIds.Contains(x.CourseID))
                .Select(x => x.StudentID)
                .Distinct()
                .Count();

            int lessonsCount = _context.Lessons
                .Count(x => courseIds.Contains(x.CourseID));

            int materialsCount = _context.CourseMaterials
                .Count(x => courseIds.Contains(x.CourseID));

            ViewBag.CoursesCount = coursesCount;
            ViewBag.StudentsCount = studentsCount;
            ViewBag.LessonsCount = lessonsCount;
            ViewBag.MaterialsCount = materialsCount;

            ViewBag.Courses = courses;

            return View();
        }


        // =========================
        // My Courses
        // =========================
        public IActionResult MyCourses()
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var courses = _context.Courses
                .Where(x => x.TeacherID == teacherId.Value)
                .ToList();

            return View(courses);
        }


        // =========================
        // Manage Course
        // =========================
        public IActionResult ManageCourse(int id)
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var course = _context.Courses
                .FirstOrDefault(x =>
                    x.CourseID == id &&
                    x.TeacherID == teacherId.Value);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }


        // =========================
        // All Students Of Teacher
        // =========================
        public IActionResult Students()
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var courseIds = _context.Courses
                .Where(x => x.TeacherID == teacherId.Value)
                .Select(x => x.CourseID)
                .ToList();

            var students = _context.Enrollments
                .Where(x => courseIds.Contains(x.CourseID))
                .Join(
                    _context.Users,
                    enrollment => enrollment.StudentID,
                    user => user.UserID,
                    (enrollment, user) => user
                )
                .Where(x => x.Role == "Student")
                .Distinct()
                .ToList();

            return View(students);
        }


        // =========================
        // Students Of One Course
        // =========================
        public IActionResult CourseStudents(int courseId)
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var course = _context.Courses
                .FirstOrDefault(x =>
                    x.CourseID == courseId &&
                    x.TeacherID == teacherId.Value);

            if (course == null)
            {
                return NotFound();
            }

            var students = _context.Enrollments
                .Where(x => x.CourseID == courseId)
                .Join(
                    _context.Users,
                    enrollment => enrollment.StudentID,
                    user => user.UserID,
                    (enrollment, user) => user
                )
                .Where(x => x.Role == "Student")
                .Distinct()
                .ToList();

            ViewBag.Course = course;

            return View("Students", students);
        }


        // =========================
        // Reports
        // =========================
        public IActionResult Reports()
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var courses = _context.Courses
                .Where(x => x.TeacherID == teacherId.Value)
                .ToList();

            int coursesCount = courses.Count;

            var courseIds = courses
                .Select(x => x.CourseID)
                .ToList();

            int studentsCount = _context.Enrollments
                .Where(x => courseIds.Contains(x.CourseID))
                .Select(x => x.StudentID)
                .Distinct()
                .Count();

            int lessonsCount = _context.Lessons
                .Count(x => courseIds.Contains(x.CourseID));

            int materialsCount = _context.CourseMaterials
                .Count(x => courseIds.Contains(x.CourseID));

            ViewBag.CoursesCount = coursesCount;
            ViewBag.StudentsCount = studentsCount;
            ViewBag.LessonsCount = lessonsCount;
            ViewBag.MaterialsCount = materialsCount;

            return View(courses);
        }


        // =========================
        // Lessons
        // =========================
        public IActionResult Lessons(int id)
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var course = _context.Courses
                .FirstOrDefault(x =>
                    x.CourseID == id &&
                    x.TeacherID == teacherId.Value);

            if (course == null)
            {
                return NotFound();
            }

            var lessons = _context.Lessons
                .Where(x => x.CourseID == id)
                .OrderBy(x => x.LessonNumber)
                .ThenBy(x => x.LessonID)
                .ToList();

            ViewBag.Course = course;

            return View(lessons);
        }


        // =========================
        // Create Lesson - GET
        // =========================
        [HttpGet]
        public IActionResult CreateLesson(int courseId)
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var course = _context.Courses
                .FirstOrDefault(x =>
                    x.CourseID == courseId &&
                    x.TeacherID == teacherId.Value);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;

            return View();
        }


        // =========================
        // Create Lesson - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLesson(
            int courseId,
            string title,
            string description,
            string videoUrl)
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var course = _context.Courses
                .FirstOrDefault(x =>
                    x.CourseID == courseId &&
                    x.TeacherID == teacherId.Value);

            if (course == null)
            {
                return NotFound();
            }

            int lessonNumber = _context.Lessons
                .Where(x => x.CourseID == courseId)
                .Select(x => (int?)x.LessonNumber)
                .Max() ?? 0;

            var lesson = new Lesson
            {
                CourseID = courseId,
                Title = title,
                Description = description,
                VideoUrl = videoUrl,
                LessonNumber = lessonNumber + 1,
                CreatedDate = DateTime.Now
            };

            _context.Lessons.Add(lesson);
            _context.SaveChanges();

            return RedirectToAction(
                "Lessons",
                new { id = courseId }
            );
        }


        // =========================
        // Course Materials
        // =========================
        public IActionResult CourseMaterials(int id)
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var course = _context.Courses
                .FirstOrDefault(x =>
                    x.CourseID == id &&
                    x.TeacherID == teacherId.Value);

            if (course == null)
            {
                return NotFound();
            }

            var materials = _context.CourseMaterials
                .Where(x => x.CourseID == id)
                .OrderBy(x => x.CourseMaterialID)
                .ToList();

            ViewBag.Course = course;

            return View(materials);
        }


        // =========================
        // Create Course Material - GET
        // =========================
        [HttpGet]
        public IActionResult CreateCourseMaterial(int courseId)
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var course = _context.Courses
                .FirstOrDefault(x =>
                    x.CourseID == courseId &&
                    x.TeacherID == teacherId.Value);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;

            return View();
        }


        // =========================
        // Create Course Material - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCourseMaterial(
            int courseId,
            string title,
            string description,
            string materialType,
            IFormFile file)
        {
            int? teacherId = HttpContext.Session.GetInt32("UserID");

            if (teacherId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var course = _context.Courses
                .FirstOrDefault(x =>
                    x.CourseID == courseId &&
                    x.TeacherID == teacherId.Value);

            if (course == null)
            {
                return NotFound();
            }

            string fileName = null;
            string filePath = null;

            if (file != null && file.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "course-materials"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                fileName = Guid.NewGuid().ToString() +
                           Path.GetExtension(file.FileName);

                string physicalPath = Path.Combine(
                    uploadsFolder,
                    fileName
                );

                using (var stream = new FileStream(
                    physicalPath,
                    FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                filePath = "/uploads/course-materials/" + fileName;
            }

            var material = new CourseMaterial
            {
                CourseID = courseId,
                Title = title,
                Description = description,
                FileName = file?.FileName,
                FilePath = filePath,
                MaterialType = materialType,
                CreatedDate = DateTime.Now
            };

            _context.CourseMaterials.Add(material);
            _context.SaveChanges();

            return RedirectToAction(
                "CourseMaterials",
                new { id = courseId }
            );
        }
    }
}