using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Praetor.Setup

{
    public partial class CreateDatabase : Form
    {
        public string Info
        {
            get { return infoLabel.Text; }
            set { infoLabel.Text = value; }
        }
        public string HostName
        {
            get { return serverBox.Text; }
            set { serverBox.Text = value; }
        }
        public string DatabaseName
        {
            get { return databaseBox.Text; }
            set { databaseBox.Text = value; }
        }
        public string Username
        {
            get { return loginBox.Text; }
            set { loginBox.Text = value; }
        }
        public string Password1
        {
            get { return passwordBox1.Text; }
            set { passwordBox1.Text = value; }
        }
        public string Password2
        {
            get { return passwordBox2.Text; }
            set { passwordBox2.Text = value; }
        }

        private bool create = true;

        private List<string> scripts = new List<string>() { "create.sql", "init.sql" };

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            if (!validate())
                return;

            if (!create)
            {
                saveConfig();
                MessageBox.Show("Settings have been saved successfully", "OK");
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else if (passwordBox1.Text == passwordBox2.Text)
            {
                DbCreator db = new DbCreator(HostName,  DatabaseName, Username, Password1);
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                foreach (var s in scripts)
                {
                    try
                    {
                        db.AddScript(File.ReadAllText(Path.Combine(path, s)));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Database script {0} could not be read. Error: {1}", s, ex.Message), "Error");
                    }
                }

                try
                {
                    if (db.Create())
                    {
                        saveConfig();
                        MessageBox.Show("Database has been created successfully", "OK");
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        this.Enabled = true;
                        MessageBox.Show("Database could not be created", "Error");
                    }
                }
                catch (Exception ex)
                {
                    this.Enabled = true;
                    MessageBox.Show(string.Format("Database could not be created. Error: {0}", ex.Message), "Error");
                }
            }
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool validate()
        {
            if (string.IsNullOrWhiteSpace(HostName))
            {
                MessageBox.Show("Provide name of database server instance (for example: .\\SQLEXPRESS)");
                return false;
            }
            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                MessageBox.Show("Provide name of database (for example: mts)");
                return false;
            }
            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show("Provide username that will be used to access database");
                return false;
            }
            if (string.IsNullOrEmpty(Password1))
            {
                MessageBox.Show("Provide password that will be used to access database");
                return false;
            }
            if (create && Password1 != Password2)
            {
                MessageBox.Show("Passwords are not equal");
                return false;
            }

            return true;
        }

        private void existingDbButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton b = sender as RadioButton;
            if (b != null)
            {
                create = !b.Checked;
                passwordBox2.Visible = create;
                passwordLabel2.Visible = create;
            }
        }

        private void saveConfig()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..\\MTS.exe");
            string connName = "MTSContext";
            string connStr = "metadata=res://*/MTSModel.csdl|res://*/MTSModel.ssdl|res://*/MTSModel.msl;provider=System.Data.SqlClient;provider connection string=\"data source={0};initial catalog={1};Persist Security Info=True;User ID={2};Password={3};MultipleActiveResultSets=True;Application Name=EntityFramework;User Instance=False\"";
            string provider = "System.Data.EntityClient";


            // load MTS application configuration file
            Configuration conf = ConfigurationManager.OpenExeConfiguration(path);
            // if such a connection string already exists - remove it and add a new one
            if (conf.ConnectionStrings.ConnectionStrings[connName] != null)
                conf.ConnectionStrings.ConnectionStrings.Remove(connName);
            
            // add new connection string according information provided by user
            conf.ConnectionStrings.ConnectionStrings.Add(
                new ConnectionStringSettings(connName, 
                    string.Format(connStr, HostName, DatabaseName, Username, Password1),
                    provider));

            conf.Save(ConfigurationSaveMode.Modified);
        }

        #region Constructors

        public CreateDatabase()
        {
            InitializeComponent();
        }

        #endregion
    }
}
