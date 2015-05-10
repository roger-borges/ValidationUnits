using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ValidationUnits
{
    public class ValidatorSettings
    {
        /// <summary>
        /// Each property's default value is true
        /// </summary>
        public ValidatorSettings()
        {
            UseDataAnnotationValidator = true;
            IncludeChildrenInDataAnnotationValidation = true;
            StopValidatingAtFirstError = true;
        }

        public static ValidatorSettings CreateDefaultValidatorSettings()
        {
            return new ValidatorSettings();
        }

        /// <summary>
        /// Default value is true
        /// </summary>
        public bool UseDataAnnotationValidator { get; set; }

        /// <summary>
        /// Default value is true
        /// </summary>
        public bool IncludeChildrenInDataAnnotationValidation { get; set; }

        /// <summary>
        /// Default value is true
        /// </summary>
        public bool StopValidatingAtFirstError { get; set; }
    }
}
