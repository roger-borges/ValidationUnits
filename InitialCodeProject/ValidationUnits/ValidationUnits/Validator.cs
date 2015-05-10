using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValidationUnits.BusinessValidationUnits;

namespace ValidationUnits
{
    public static class Validator
    {
        public static ValidationUnitResult Validate<T>(T entity, ValidatorSettings settings, params IBusinessValidationUnit<T>[] validators) where T : class
        {
            var validatorsList = new List<IBusinessValidationUnit<T>>();
            if(settings.UseDataAnnotationValidator)
            {
                validatorsList.Add(new DataAnnotationsValidationUnit<T>(settings.IncludeChildrenInDataAnnotationValidation));
            }
            validatorsList.AddRange(validators);

            var returnValue = new ValidationUnitResult()
            {
                ErrorMessages = new List<string>()
            };

            foreach (var v in validatorsList)
            {
                 returnValue.ErrorMessages.AddRange(v.Validate(entity).ErrorMessages);

            }
            return returnValue;
        }
    }
}
