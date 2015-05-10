using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ValidationUnits
{
    public class ValidationUnitResult
    {
        public ValidationUnitResult()
        {
            ErrorMessages = new List<string>();
        }

        public bool HasErrors { get { return ErrorMessages != null && ErrorMessages.Count > 0; } }

        public List<string> ErrorMessages { get; set; }
    }
}
