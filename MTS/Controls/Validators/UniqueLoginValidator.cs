using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using MTS.Data;

namespace MTS.Controls.Validators
{
    public class UniqueLoginValidator : ValidationRule
    {
        public string MyLogin { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "This field is required");
            }
            else if (value is string)
            {
                if (string.IsNullOrWhiteSpace(value as string))
                    return new ValidationResult(false, "This field is required");
            }

            using (MTSContext context = new MTSContext())
            {
                string login = value as string;
                bool isUnique = context.Operators.FirstOrDefault(o => o.Login != MyLogin && o.Login == login) == null;

                if (!isUnique)
                {
                    return new ValidationResult(false, string.Format("Login \"{0}\" already exists", login));
                }
            }

            return new ValidationResult(true, null);
        }
    }
}
