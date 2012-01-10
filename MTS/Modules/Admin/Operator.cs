using System;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using MTS.Data;
using MTS.Data.Types;

namespace MTS.Admin
{
    /// <summary>
    /// Singelton: Provide access to logged in operator and allow operations such as log in and log out
    /// </summary>
    public class Operator
    {
        private static Operator instance = null;
        /// <summary>
        /// Singelton instance of operator
        /// </summary>
        public static Operator Instance { get { return instance; } }

        /// <summary>
        /// Compute hexadecimal string from given password
        /// </summary>
        /// <param name="password">Password string to compute hash from</param>
        /// <returns>String containing hexadecimal representation of hash value</returns>
        public static string ComputeHash(string password)
        {
            StringBuilder passHash = new StringBuilder(64); // 64 is length of SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(password);
                buffer = sha256.ComputeHash(buffer);
                passHash.Clear();
                // convert each byte of hash to string hexadecimal representation
                for (int i = 0; i < buffer.Length; i++)
                    passHash.Append(string.Format("{0:x2}", buffer[i]));
            }
            return passHash.ToString();
        }
        /// <summary>
        /// Try to log in given user without throwing any exception. If user can not be logged in false is returned
        /// </summary>
        /// <param name="login">Login of user to log in</param>
        /// <param name="password">Password of user to log in</param>
        /// <returns>Logical value indicating if login process was succesfull</returns>
        public static bool TryLogin(string login, string password)
        {
            string passHash = ComputeHash(password);    // convert given password to hash

            bool result = false;    // value indicating if login process was succesfull
            try
            {   // catch database exceptions
                using (MTSContext context = new MTSContext())
                {   // connect to database and find such an operator with given login and password - do not throw
                    // exception if such an operator does not exists
                    Data.Operator op = context.Operators.FirstOrDefault(o => o.Login == login && o.Password == passHash);
                    if (op != null)
                    {   // create an instance of object holding information about loged in user
                        instance = new Operator
                        {
                            Name = op.Name,
                            Surname = op.Surname,
                            Login = op.Login,
                            Id = op.Id,             // necessary when referencing operator (database id)
                            Type = (OperatorType)op.Type    // for permission only
                        };
                        result = true;              // operator was loged in succesfully
                    }
                }
            }
            catch (Exception ex)
            {   // an exception has been thrown while connecting to database - user can not be loged in
                ExceptionManager.ShowError(ex);
                result = false;
            }

            return result;
        }
        /// <summary>
        /// Logout operator. Singelton instance will be deleted. At the time of calling this method all tabs must be
        /// closed.
        /// </summary>
        public static void LogOut()
        {
            instance = null;
        }
        /// <summary>
        /// Get value indicating whether some operator is logged in
        /// </summary>
        /// <returns>True if operator is logged in (<see cref="Operator.Instance"/> is not null)</returns>
        public static bool IsLoggedIn()
        {
            return instance != null;
        }
        /// <summary>
        /// Get value indicatin wheter logged in operator is in given role. Return false if no operator is logged in.
        /// </summary>
        /// <param name="role">Operator role to test if operator is in</param>
        /// <returns>True if some operator is logged in and is in given role</returns>
        public static bool IsInRole(OperatorType role)
        {
            if (IsLoggedIn())
                return instance.Type == role;
            return false;
        }
        public static void DebugLogin()
        {
            instance = new Operator()
            {
                Name = "System",
                Surname ="Administrator",
                Login = "admin",
                Id = 1,
                Type = OperatorType.Admin
            };
        }

        #region Properties

        /// <summary>
        /// (Get) Operator first name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// (Get) Operator seconde name
        /// </summary>
        public string Surname { get; private set; }
        /// <summary>
        /// (Get) Operator full name. Concatenation of <see cref="Name"/> and <see cref="Surname"/>
        /// </summary>
        public string FullName { get { return string.Join(" ", Name, Surname); } }
        /// <summary>
        /// (Get) Operator login
        /// </summary>
        public string Login { get; private set; }
        /// <summary>
        /// (Get) Operator database id. Primary key in Operator table
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// (Get) Operator type or role. This type defines operator priviledges
        /// </summary>
        public OperatorType Type { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Operator instance can be only created in static method
        /// </summary>
        private Operator() { }

        #endregion
    }
}
