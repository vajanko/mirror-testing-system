using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;
using System.IO;


namespace MTS
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        #region Install

        public override void Install(IDictionary stateSaver)
        {
            Properties.Settings.Default.AppDir = Path.GetDirectoryName(Context.Parameters["assemblypath"]);
            // start database installer
            string target = Path.GetDirectoryName(Context.Parameters["assemblypath"]);
            string procPath = Path.Combine(target, "install\\DbInstaller.exe");

            Process proc = Process.Start(procPath);
            proc.WaitForExit();
            
            base.Install(stateSaver);
        }

        #endregion

        #region Commit

        public override void Commit(IDictionary savedState)
        {   // delete installation directory
            try
            {
                string target = Path.GetDirectoryName(Context.Parameters["assemblypath"]);
                Directory.Delete(Path.Combine(target, "install"), true);
            }
            catch (Exception ex)
            {
                // do not care about this - could be written to installation log
            }

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

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
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
