using System;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class CourseMaterial
    {
        [Key]
        public int CourseMaterialID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string MaterialType { get; set; }

        public int CourseID { get; set; }

        public DateTime CreatedDate { get; set; }
    }

}