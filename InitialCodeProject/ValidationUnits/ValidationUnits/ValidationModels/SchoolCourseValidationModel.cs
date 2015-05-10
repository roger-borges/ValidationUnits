using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ValidationUnits.ValidationModels
{
    public class SchoolCourseValidationModel
    {
        public Course Course { get; set; }

        public Class[] Classes { get; set; }
    }
}
