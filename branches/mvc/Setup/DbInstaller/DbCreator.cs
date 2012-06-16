using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace MTS.Setup
{
    class DbCreator
    {
        private List<string> scripts = new List<string>();
        private string host;
        private string databaseName;
        private string username;
        private string password;

        public static void CreateLogin(string host, string login, string password)
        {            
            SqlConnection con = new SqlConnection(string.Format("Data Source={0};Integrated Security=True;", host));
            ServerConnection sc = new ServerConnection(con);
            Server server = new Server(sc);
            Login newLogin = new Login(server, login);
            newLogin.LoginType = LoginType.SqlLogin;
            newLogin.Create(password, LoginCreateOptions.None);
        }

        public void AddScript(string script)
        {
            scripts.Add(script);
        }

        public bool Create()
        {
            bool result = false;
            string connStr = string.Format("Data Source={0};Integrated Security=True;", host);
            SqlConnection conn = new SqlConnection(connStr);
            ServerConnection sc = new ServerConnection(conn);
            Server server = new Server(sc);

            // check is such a database already exists and if not create it
            Database database = server.Databases[databaseName];
            if (database == null)
            {   // create new database
                database = new Database(server, databaseName);
                
                database.Create();

                // check if login already exists
                Login login = server.Logins[username];
                if (login == null)
                {   // create a new login for this application
                    login = new Login(server, username);
                    login.DefaultDatabase = databaseName;
                    login.LoginType = LoginType.SqlLogin;
                    
                    login.Create(password, LoginCreateOptions.None);
                }

                // create a new user for this database
                User user = new User(database, username);
                user.Login = username;
                user.Create();
                // grant user access to the database
                user.AddToRole("db_owner");
                
                // use just create database for executing scripts, need to reconnect to do this
                // this is not working !!!
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "USE  @dbName";
                cmd.Parameters.Add(new SqlParameter("@dbName", SqlDbType.VarChar, 30));
                cmd.Parameters["@dbName"].Value = databaseName;
                //cmd.ExecuteNonQuery();

                // execute given scripts
                foreach (string script in scripts)
                    server.ConnectionContext.ExecuteNonQuery(script);
                result = true;
            }
            else
            {
                MessageBox.Show(string.Format("Database \"{0}\" already exists. Please select another name", databaseName), "Database");
            }
            conn.Close();   // close the connection

            return result;
        }

        public void Drop()
        {
            // connect to database server
            ServerConnection sc = new ServerConnection(host, username, password);
            var server = new Microsoft.SqlServer.Management.Smo.Server(sc);

            // check is such a database already exists and if drop it
            Database database = server.Databases[databaseName];

            if (database != null)
            {
                server.KillAllProcesses(databaseName);
                server.KillDatabase(databaseName);
            }
        }

        public DbCreator(string dbHost, string dbName, string dbUser, string dbPassword)
        {
            host = dbHost;
            databaseName = dbName;
            username = dbUser;
            password = dbPassword;
        }
    }
}
