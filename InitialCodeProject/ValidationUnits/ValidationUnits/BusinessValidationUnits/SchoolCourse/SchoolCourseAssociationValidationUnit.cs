using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValidationUnits.ValidationModels;

namespace ValidationUnits.BusinessValidationUnits.SchoolCourse
{
    public class SchoolCourseAssociationValidationUnit : IBusinessValidationUnit<SchoolCourseValidationModel>
    {
        public ValidationUnitResult Validate(SchoolCourseValidationModel model)
        {
            if (model.Classes.Length != model.Course.Classes.Count)
            {
                return new ValidationUnitResult()
                {
                    ErrorMessages = new List<string>()
                    {
                        "Your course and its classes are not properly assigned to each other"
                    }
                };
            }
            else if (model.Classes.Any(x => x.Course.CourseID != model.Course.CourseID))
            {
                return new ValidationUnitResult()
                {
                    ErrorMessages = new List<string>()
                    {
                        "At least one of your course's classes do not properly have their parent course assigned"
                    }
                };
            }
            return new ValidationUnitResult()
            {
                ErrorMessages = new List<string>()
            };
        }
    }
}
