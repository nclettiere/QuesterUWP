using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Quester.Helper
{
    class IOHelper
    {
        public static string GetUserHomeDir()
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }
            return path;
        }

        public static string GetDefaultProjectDir()
        {
            string exe = Path.Combine(GetUserHomeDir(), "QuesterProjects");
            return exe;
        }

        public static string FormatProjectPath(string projectName)
        {
            if(!String.IsNullOrEmpty(projectName))
                return Path.Combine(GetDefaultProjectDir(), projectName);
            return "";
        }

        public static string FormatProjectPath(string projectName, string customPath)
        {
            if (!String.IsNullOrEmpty(projectName) && !String.IsNullOrEmpty(customPath))
                return Path.Combine(customPath, projectName);

            return "";
        }
    }
}
