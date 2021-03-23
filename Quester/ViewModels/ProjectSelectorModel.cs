using Quester.Data;
using Quester.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quester.ViewModels
{
    public class ProjectSelectorModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Project> Projects { get; set; }
        public int ProjectsCount { get; set; }
        public bool LoadingProjects { get; set; }

        private ICommand reloadClick;

        public ProjectSelectorModel()
        {
            ProjectsCount = 0;
            LoadingProjects = false;
            Projects = new ObservableCollection<Project>();
            RetrieveAll();
        }

        public ICommand ReloadClick
        {
            get
            {
                return reloadClick ?? (reloadClick = new CommandHandler(() => RetrieveAll(), () => CanExecute));
            }
        }
        public bool CanExecute
        {
            get
            {
                return true;
            }
        }


        public void AddItem(ref Project project)
        {
            if (!LoadingProjects)
            {
                LoadingProjects = true;
                Projects.Add(project);
                ProjectsCount++;
                LoadingProjects = false;
                OnPropertyChanged("Projects");
                OnPropertyChanged("ProjectsCount");
                OnPropertyChanged("LoadingProjects");
            }
        }
        public void Replace(ref ObservableCollection<Project> projects)
        {
            if (!LoadingProjects)
            {
                LoadingProjects = true;
                Projects = projects;
                ProjectsCount = Projects.Count;
                LoadingProjects = false;
                OnPropertyChanged("Projects");
                OnPropertyChanged("ProjectsCount");
                OnPropertyChanged("LoadingProjects");
            }
        }

        public async void RetrieveAll()
        {
            if (!LoadingProjects)
            {
                LoadingProjects = true;
                IReadOnlyList<string> projectFiles = await ProjectHelper.SearchForProjects();

                Projects.Clear();

                foreach (string pFile in projectFiles)
                {
                    Projects.Add(await Project.GetProjectFromJsonFile(pFile));
                    ProjectsCount++;
                }
                LoadingProjects = false;

                OnPropertyChanged("Projects");
                OnPropertyChanged("ProjectsCount");
                OnPropertyChanged("LoadingProjects");
            }
        }
    }
}
