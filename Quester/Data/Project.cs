using Newtonsoft.Json;
using Quester.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Quester.Data
{
    public class Project
    {
        public string Name { get; set; }
        public string ProjectPath { get; set; }
        public string ProjectDescription { get; set; }
        public string QuesterVersion { get; set; }
        public IList<Engines> Engines { get; set; }
        public IList<string> EnginesPath { get; set; }

        public Project()
        {
            Engines = new List<Engines>();
            EnginesPath = new List<string>();
        }

        public Project(string name, string projectPath, string projectDescription, string questerVersion, List<Engines> engines, List<string> enginesPath)
        {
            Name = name;
            ProjectPath = projectPath;
            ProjectDescription = projectDescription;
            QuesterVersion = questerVersion;
            Engines = engines;
            EnginesPath = enginesPath;
        }

        internal void FillNewData(NewProjectData pData)
        {
            Name = pData.ProjectName;
            ProjectPath = pData.ProjectPath;
            ProjectDescription = pData.ProjectDesc;
            //QuesterVersion = <TODO>;
            //Engines = <TODO>;
            //EnginesPath = <TODO>;
        }

        internal async Task<bool> SaveProjectAsync()
        {
            if (String.IsNullOrEmpty(ProjectPath))
                return false;

            return await ProjectHelper.SerializeProjectJSON(this);
        }
        internal string GetProjectFileFormated()
        {
            if (String.IsNullOrEmpty(ProjectPath))
                return String.Empty;

            return Path.Combine(ProjectPath, ProjectHelper.FormatProjectName(Name) + ".qter");
        }

        internal string GetProjectFileName()
        {
            if (String.IsNullOrEmpty(ProjectPath))
                return String.Empty;

            return (ProjectHelper.FormatProjectName(Name) + ".qter");
        }

        internal (string, string) GetProjectCharactersFolder()
        {
            if (String.IsNullOrEmpty(ProjectPath))
                return (String.Empty, String.Empty);

            return ((ProjectHelper.FormatProjectName(Name) + @"\Characters"), (ProjectHelper.FormatProjectName(Name) + @"\Characters\DB"));
        }

        internal (string, string) GetProjectDialoguesFolder()
        {
            if (String.IsNullOrEmpty(ProjectPath))
                return (String.Empty, String.Empty);

            return ((ProjectHelper.FormatProjectName(Name) + @"\Dialogues"), (ProjectHelper.FormatProjectName(Name) + @"\Dialogues\DB"));
        }

        internal (string, string) GetProjectQuestsFolder()
        {
            if (String.IsNullOrEmpty(ProjectPath))
                return (String.Empty, String.Empty);

            return ((ProjectHelper.FormatProjectName(Name) + @"\Quests"), (ProjectHelper.FormatProjectName(Name) + @"\Quests\DB"));
        }

        public static async Task<Project> GetProjectFromJsonFile(string pFile)
        {
            try
            {
                StorageFile DefaultFile = await StorageFile.GetFileFromPathAsync(pFile);
                string Json = await FileIO.ReadTextAsync(DefaultFile);

                return JsonConvert.DeserializeObject<Project>(Json);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }
    }

    public enum Engines
    {
        Unreal,
        Unity,
        Godot
    }
}
