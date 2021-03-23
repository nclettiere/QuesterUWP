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
        public static async Task<bool> CreateNewProject(FrameworkElement context, NewProjectData pData)
        {
            Uri ProjectTemplate = new Uri(context.BaseUri, "/Assets/template_default.json");
            StorageFile DefaultFile = StorageFile.GetFileFromApplicationUriAsync(ProjectTemplate).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            string Json = FileIO.ReadTextAsync(DefaultFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            Project NewProject = JsonConvert.DeserializeObject<Project>(Json);
            NewProject.FillNewData(pData);

            return await NewProject.SaveProjectAsync();
        }

        public static string FormatProjectName(string projectName)
        {
            return projectName.Replace(" ", "_");
        }

        public async static Task<bool> CreateDefaultProjectFolders(Project project)
        {
            if (project.ProjectPath.Contains(IOHelper.GetDefaultProjectDir()))
            {
                var (CharactersFolder, CharactersDBFolder) = project.GetProjectCharactersFolder();
                var (DialoguesFolder, DialoguesDBFolder) = project.GetProjectDialoguesFolder();
                var (QuestsFolder, QuestsDBFolder) = project.GetProjectQuestsFolder();
                StorageFolder sf = await IOHelper.GetProjectsFolder();
                await sf.CreateFolderAsync(CharactersDBFolder, CreationCollisionOption.OpenIfExists);
                await sf.CreateFolderAsync(DialoguesDBFolder, CreationCollisionOption.OpenIfExists);
                await sf.CreateFolderAsync(QuestsDBFolder, CreationCollisionOption.OpenIfExists);
                return true;
            }else
            {
                /// TODO
            }

            return false;
        }

        public async static Task<bool> SerializeProjectJSON(Project project) 
        {
            try
            {
                if(project.ProjectPath.Equals(IOHelper.GetDefaultProjectDir())) {
                    StorageFolder ProjectsFolder = await IOHelper.GetProjectsFolder();
                    if (await IOHelper.CreateFolder(ProjectsFolder, FormatProjectName(project.Name)))
                    {
                        project.ProjectPath = IOHelper.FormatProjectPath(project.Name);

                        string mainFileStr = project.GetProjectFileName();
                        if (mainFileStr == String.Empty) return false;

                        StorageFile ProjectMainFile = await IOHelper.CreateFile(project.ProjectPath, mainFileStr);
                        if (ProjectMainFile == null) return false;

                        await Windows.Storage.FileIO.WriteTextAsync(ProjectMainFile, JsonConvert.SerializeObject(project, Formatting.Indented));
                        await CreateDefaultProjectFolders(project);

                        return true;

                    }
                }

                /// TODO
                return false;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public static async Task<IReadOnlyList<string>> SearchForProjects(string customLocation = null)
        {
            List<string> pFiles = new List<string>();
            if(customLocation == null)
            {
                StorageFolder ProjectsFolder = await IOHelper.GetProjectsFolder();
                IReadOnlyList<StorageFolder> folderList = await ProjectsFolder.GetFoldersAsync();

                foreach (StorageFolder folder in folderList)
                {
                    IReadOnlyList<StorageFile> projectFiles = await folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByName);
                    var List = new List<string>() { ".qter", ".quester", ".qtr" };
                    var ProjectMain = projectFiles.Where(f => List.Any(e => e == Path.GetExtension(f.FileType)))
                              .Select(f => f.Path);

                    pFiles.Add(ProjectMain.First());
                }
            }

            return pFiles;
        }
    }
}
