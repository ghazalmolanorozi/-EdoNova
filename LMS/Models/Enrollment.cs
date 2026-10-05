using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }

        public int StudentID { get; set; }

        public int CourseID { get; set; }
    }
}