using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
    public class Course
    {
        public int CourseID { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public List<Class> Classes { get; set; }
    }
}
