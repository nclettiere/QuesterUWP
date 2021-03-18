using Newtonsoft.Json;
using Quester.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using static Quester.Data.Project;

namespace Quester.Helper
{
    public struct NewProjectData
    {
        public NewProjectData(string projectName, string projectPath)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            ProjectDesc = String.Empty;
        }

        public NewProjectData(string projectName, string projectPath, string projectDesc)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            ProjectDesc = projectDesc;
        }

        public string ProjectName { get; }
        public string ProjectPath { get; }
        public string ProjectDesc { get; }

        public override string ToString() => $"projectName({ProjectName}), projectPath({ProjectPath}), projectDesc({ProjectDesc})";
    }

    class ProjectHelper
    {
        public static bool CreateNewProject(FrameworkElement context, NewProjectData pData)
        {
            Uri ProjectTemplate = new Uri(context.BaseUri, "/Assets/template_default.json");
            StorageFile DefaultFile = StorageFile.GetFileFromApplicationUriAsync(ProjectTemplate).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            string Json = FileIO.ReadTextAsync(DefaultFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            Project NewProject = JsonConvert.DeserializeObject<Project>(Json);
            NewProject.FillNewData(pData);
            NewProject.SaveProject();

            return true;
        }

        public static bool SerializeProjectJSON(in Project project) 
        {
            try
            {
                using (StreamWriter file = File.CreateText(project.ProjectPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(file, project);
                }
            }catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }
    }
}
