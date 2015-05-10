using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ValidationUnits.BusinessValidationUnits.Classes
{
    public class ClassHasCourseValidationUnit : IBusinessValidationUnit<Class>
    {
        public ValidationUnitResult Validate(Class entity)
        {
            var errors = new List<string>();

            if (entity.Course == null)
            {
                errors.Add("The class must be associated with a course");
            }
            return new ValidationUnitResult()
            {
                ErrorMessages = errors
            };
        }
    }
}
