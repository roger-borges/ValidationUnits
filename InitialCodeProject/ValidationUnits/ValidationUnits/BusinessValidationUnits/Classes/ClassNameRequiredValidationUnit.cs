using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ValidationUnits.BusinessValidationUnits.Classes
{
    public class ClassNameRequiredValidationUnit : IBusinessValidationUnit<Class>
    {
        public ValidationUnitResult Validate(Class entity)
        {
            var errors = new List<string>();
            if(string.IsNullOrWhiteSpace(entity.Name))
            {
                errors.Add("Class Name is required");
            }

            return new ValidationUnitResult()
            {
                ErrorMessages = errors
            };
        }
    }
}
