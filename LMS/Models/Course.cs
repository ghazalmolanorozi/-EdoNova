using System;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int Capacity { get; set; }

        public DateTime CreatedDate { get; set; }
        public int TeacherID { get; set; }
    }
}