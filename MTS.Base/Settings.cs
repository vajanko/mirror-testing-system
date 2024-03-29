﻿using System;
using System.Collections.Generic;
using System.IO;

namespace MTS.Base.Properties
{
    internal sealed partial class Settings
    {
        /// <summary>
        /// Get absolute system path to the directory from which this application was executed
        /// </summary>
        /// <returns></returns>
        public string GetExecutingDirectory()
        {   // get directory part from executing assembly path
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        /// <summary>
        /// Get absolute path to directory where configuration files for this application are stored
        /// In configuration directory are stored, for example: template for editor, configuration of
        /// channels, ...
        /// </summary>
        /// <returns></returns>
        public string GetConfigDirectory()
        {
            // this should be used when in production mode
            //string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            //string config = Path.Combine(
            //    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            //    Default.AppName,
            //    Default.ConfigDir);


            // if in application settings absolute path to configuration directory is defined, just return
            // otherwise get path to executing directory and combine it with relative configuration 
            // directory from application settings
            if (Path.IsPathRooted(Default.ConfigDir))
                return Default.ConfigDir;
            else
                return Path.Combine(GetExecutingDirectory(), Default.ConfigDir);
        }
        /// <summary>
        /// Get absolute path to file where logs are stored
        /// </summary>
        /// <returns>Absolute log file path</returns>
        public string GetLogFilePath()
        {
            return Path.Combine(GetConfigDirectory(), Default.LogFile);
        }
    }
}

