using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ValidationUnits
{

    public interface IBusinessValidationUnit<T>
    {
        ValidationUnitResult Validate(T model);
    }
}
