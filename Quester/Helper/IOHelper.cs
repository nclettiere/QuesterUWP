using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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
            string combinedPath = Path.Combine(
                ApplicationData.Current.LocalFolder.Path, "Projects");

            return combinedPath;

        }

        public static async Task<StorageFolder> GetProjectsFolder()
        {
            string combinedPath = Path.Combine(
                ApplicationData.Current.LocalFolder.Path, "Projects");

            EnsureProjectStructure();

            return await StorageFolder.GetFolderFromPathAsync(combinedPath);
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

            return customPath;
        }

        // Default projects dir
        public static bool IsProjectAvailable(string projectDir)
        {
            if (!String.IsNullOrWhiteSpace(projectDir))
            {
                return !Directory.Exists(projectDir);
            }

            return false;
        }

        public async static void EnsureProjectStructure()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            
            StorageFolder folder = await storageFolder.CreateFolderAsync("Projects",
                CreationCollisionOption.OpenIfExists);
        }

        public async static Task<StorageFile> CreateFile(string folderPath, string filename)
        {
            EnsureProjectStructure();

            //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            try
            {
                StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(folderPath);
                StorageFile file = await sf.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);
                return file;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }

        }

        public async static Task<StorageFolder> CreateFolder(string path)
        {
            EnsureProjectStructure();

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            try
            {
                StorageFolder createdFolder = await storageFolder.CreateFolderAsync(path, CreationCollisionOption.FailIfExists);
                return createdFolder;
            }
            catch (Exception e)
            {
                // Folder exist
                return null;
            }
        }

        public async static Task<bool> CreateFolder(StorageFolder folder, string path)
        {
            EnsureProjectStructure();

            try
            {
                Debug.WriteLine(folder.Path);
                StorageFolder createdFolder = await folder.CreateFolderAsync(path, CreationCollisionOption.FailIfExists);
            }
            catch (Exception e)
            {
                // Folder exist
                return false;
            }

            return true;
        }

        public static string[] RecurseDirectory(string path, bool includeSubfolders = false)
        {
            EnsureProjectStructure();
            SearchOption opt = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            if (!Directory.Exists(path)) return new string[0];

            return Directory.GetFiles(path, "*.qter", opt);
        }
    }
}
