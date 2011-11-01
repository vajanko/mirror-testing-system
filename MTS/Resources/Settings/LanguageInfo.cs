using System;
using System.Collections.Generic;
using System.Windows;

namespace MTS.Settings
{
    public class LanguageInfo
    {
        private string name;
        private string abbr;
        private Uri path;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Abbr
        {
            get { return abbr; }
            set { abbr = value; }
        }
        public Uri Path
        {
            get { return path; }
            set { path = value; }
        }
        public string PathString
        {
            get { return path.LocalPath; }
            set { path = new Uri(@value); }
        }

        public override string ToString()
        {
            return Name;
        }

        public LanguageInfo(string name, string abbr, Uri path)
        {
            this.name = name;
            this.abbr = abbr;
            this.path = path;
        }
        public LanguageInfo() { name = ""; abbr = ""; path = null; }
    }

    public class LanguageManager
    {
        private List<LanguageInfo> languages;

        public void Add(LanguageInfo langInfo)
        {
            languages.Add(langInfo);
        }
        public LanguageInfo Get(string abbr)
        {
            foreach (LanguageInfo lang in languages)
                if (lang.Abbr == abbr) return lang;
            return null;
        }

        public LanguageManager()
        {
            languages = new List<LanguageInfo>();            
        }
    }
}
