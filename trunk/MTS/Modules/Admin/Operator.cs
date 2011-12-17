using System;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using MTS.Data;

namespace MTS.Admin
{
    /// <summary>
    /// Singelton
    /// </summary>
    public class Operator
    {

        private static Operator instance = null;
        public static Operator Instance { get { return instance; } }

        public static bool TryLogin(string login, string password)
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
                        Id = op.Id
                    };
                    result = true;
                }
            }

            return result;
        }

        #region Properties

        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Login { get; private set; }
        public int Id { get; private set; }

        #endregion

        #region Constructors

        private Operator() { }

        #endregion
    }
}
