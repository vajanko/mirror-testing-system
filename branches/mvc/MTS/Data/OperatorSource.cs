using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Security;
using MTS.Base;

namespace MTS.Data
{
    class OperatorSource : IDataErrorInfo
    {
        public string OperatorName { get; set; }
        public string OperatorSurname { get; set; }
        public string Login { get; set; }
        public SecureString Password { get; set; }
        public OperatorEnum Type { get; set; }


        #region IDataErrorInfo Members

        public string Error
        {
            get;
            private set;
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
                {
                    case "OperatorName":
                        if (string.IsNullOrWhiteSpace(OperatorName))
                            result = "Name is required field";
                        break;
                    case "OperatorSurname":
                        if (string.IsNullOrWhiteSpace(OperatorSurname))
                            result = "Surname is required field";
                        break;
                    case "Login":
                        if (string.IsNullOrWhiteSpace(Login))
                            result = "Login is required field";
                        break;
                        
                }

                return result;
            }
        }

        #endregion
    }
}
