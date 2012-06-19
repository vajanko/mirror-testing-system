using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using SMO = Microsoft.SqlServer.Management.Smo;


namespace MTS
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        #region Install

        #region Context Keys

        private const string assemblyPathKey = "assemblypath";
        /// <summary>
        /// Settings key for SQL instance name
        /// </summary>
        private const string sqlInstanceKey = "SqlInstance";

        #endregion

        #region Setting keys

        /// <summary>
        /// Settings key for installation directory
        /// </summary>
        public const string InstallDirKey = "InstallDir";
        /// <summary>
        /// Settings key for MTS attached database file
        /// </summary>
        public const string MtsDbFileKey = "MtsDbFile";


        #endregion

        #region Constants

        private const string databaseDirConst = "database";
        private const string dbFileConst = "mts.mdf";
        /// <summary>
        /// Constant string: <para>"Data Source={0};Initial Catalog=;Integrated security=true;AttachDBFileName={1};multipleactiveresultsets=True;App=EntityFramework"</para>
        /// <para>Arguments: 0: datasource, 1: attached database file name</para>
        /// </summary>
        private const string attachedDbConnStrConst = "metadata=res://*/MTSModel.csdl|res://*/MTSModel.ssdl|res://*/MTSModel.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source={0};Initial Catalog=;Integrated security=true;AttachDBFileName={1};multipleactiveresultsets=True;App=EntityFramework\"";
        private const string providerNameConst = "System.Data.EntityClient";
        private const string connStrNameConst = "MTSContext";

        #endregion

        public override void Install(IDictionary stateSaver)
        {
            string assemblyPath = Context.Parameters[assemblyPathKey];
            Properties.Settings.Default.AppDir = Path.GetDirectoryName(assemblyPath);

            Configuration conf = openConfig(assemblyPath);

            // save this value for un-installation - the directory must be deleted
            string installDir = Path.GetDirectoryName(assemblyPath);
            addSetting(conf, InstallDirKey, installDir);
            stateSaver.Add(InstallDirKey, installDir);      // this can be removed

            // SQL server instance - this is not configurable by now from installation GUI
            string sqlInstance = Context.Parameters[sqlInstanceKey];
            stateSaver.Add(sqlInstanceKey, sqlInstance);

            // path to database file
            string dbFile = Path.Combine(installDir, databaseDirConst, dbFileConst);
            // create attached db connection string
            string connStr = string.Format(attachedDbConnStrConst, sqlInstance, dbFile);
            // save connection string to configuration file
            addConnectionString(conf, connStrNameConst, connStr);
            stateSaver.Add(MtsDbFileKey, dbFile);

            // save configuration file
            saveConfig(conf);
            
            base.Install(stateSaver);
        }

        #region Configuration

        /// <summary>
        /// Open configuration file of given executable file.
        /// </summary>
        /// <param name="exeFile"></param>
        /// <returns></returns>
        private Configuration openConfig(string exeFile)
        {
            Configuration conf = null;
            int count = 3;
            while (count > 0)
            {
                try
                {
                    conf = ConfigurationManager.OpenExeConfiguration(exeFile);
                    count = 0;
                }
                catch
                {
                    count--;
                }
            }
            return conf;
        }
        private void addSetting(Configuration conf, string name, string value)
        {
            int count = 3;
            while (count > 0)
            {
                try
                {
                    conf.AppSettings.Settings.Add(name, value);
                    count = 0;
                }
                catch
                {
                    count--;
                }
            }
        }
        private void addConnectionString(Configuration conf, string connStrName, string connStr)
        {
            // if such a connection string already exists - remove it and add a new one
            if (conf.ConnectionStrings.ConnectionStrings[connStrName] != null)
                conf.ConnectionStrings.ConnectionStrings.Remove(connStrName);

            // add new connection string according information provided by user
            conf.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(connStrName, connStr, providerNameConst));
        }
        private void saveConfig(Configuration conf)
        {
            int count = 3;
            while (count > 0)
            {
                try
                {
                    conf.Save(ConfigurationSaveMode.Modified);
                    count = 0;
                }
                catch
                {
                    count--;
                }
            }
        }

        #endregion

        #endregion

        #region Commit

        #region Context Keys

        private const string runProgramKey = "runProgram";
        private const string createDesktopIconsKey = "createDesktopIcons";

        #endregion

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        #endregion

        #region Rollback

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        #endregion

        #region Uninstall

        
        private void Installer_BeforeUninstall(object sender, InstallEventArgs e)
        {
            try
            {
                string sqlInstance = e.SavedState[sqlInstanceKey] as string;
                string dbName = e.SavedState[MtsDbFileKey] as string;
                detachDatabase(sqlInstance, dbName);
            }
            catch { }
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        private void Installer_AfterUninstall(object sender, InstallEventArgs e)
        {
            try
            {
                // clean installation directory (log files, ...)
                string installDir = e.SavedState[InstallDirKey] as string;
                Directory.Delete(installDir, true);
            }
            catch { }
        }

        private void detachDatabase(string server, string dbname)
        {
            try
            {
                SMO.Server sqlInstance = new SMO.Server(server);
                sqlInstance.DetachDatabase(dbname, false);
            }
            catch { }
        }
        

        #endregion

        #region Constructors

        public Installer()
        {
            InitializeComponent();
        }

        #endregion
    }
}
