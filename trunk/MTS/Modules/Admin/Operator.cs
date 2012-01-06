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
    /// Singelton
    /// </summary>
    public class Operator
    {

        private static Operator instance = null;
        public static Operator Instance { get { return instance; } }

        public static string ComputeHash(string password)
        {
            string passHash;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(password);
                buffer = sha256.ComputeHash(buffer);
                passHash = string.Empty;
                for (int i = 0; i < buffer.Length; i++)
                    passHash += string.Format("{0:x2}", buffer[i]);
            }
            return passHash;
        }

        public static bool TryLogin(string login, string password)
        {
            string passHash = ComputeHash(password);

            bool result = false;
            using (MTSContext context = new MTSContext())
            {
                Data.Operator op = context.Operators.FirstOrDefault(o => o.Login == login && o.Password == passHash);
                if (op != null)
                {
                    instance = new Operator
                    {
                        Name = op.Name,
                        Surname = op.Surname,
                        Login = op.Login,
                        Id = op.Id,
                        Type = (OperatorType)op.Type
                    };
                    result = true;
                }
            }

            return result;
        }
        public static void LogOut()
        {
            instance = null;
        }
        public static bool IsInRole(OperatorType role)
        {
            if (instance != null)
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

        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Login { get; private set; }
        public int Id { get; private set; }
        public OperatorType Type { get; private set; }

        #endregion

        #region Constructors

        private Operator() { }

        #endregion
    }
}
