using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MTS.Controls.Validators
{
    public class RequiredFieldValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "This field is required");
            else if (value is string)
            {
                if (string.IsNullOrWhiteSpace(value as string))
                    return new ValidationResult(false, "This field is required");
            }

            return new ValidationResult(true, null);
        }
    }
}
