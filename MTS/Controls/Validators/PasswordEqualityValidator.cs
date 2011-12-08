using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MTS.Controls.Validators
{
    class PasswordEqualityValidator : ValidationRule
    {
        public string ConfirmPasswordName { get; set; }

        public PasswordBox Password { get; set; }
        public PasswordBox ConfirmPassword { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            
            if (Password != null || Password.SecurePassword != ConfirmPassword.SecurePassword)
                return new ValidationResult(false, "Passwords are not equal");

            return new ValidationResult(true, null);
        }
    }
}
