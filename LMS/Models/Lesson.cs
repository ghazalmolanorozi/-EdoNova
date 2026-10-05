using System;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class Lesson
    {
        [Key]
        public int LessonID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string VideoUrl { get; set; }

        public int CourseID { get; set; }

        public DateTime CreatedDate { get; set; }
        public int LessonNumber { get; set; }
    }

}