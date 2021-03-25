using GalaSoft.MvvmLight.Messaging;
using Quester.Controls;
using Quester.Data;
using Quester.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quester.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectViewer : Page
    {
        public bool IsProjectReady { get; set; }
        public bool LoadingProject { get; set; }
        public ProjectViewer()
        {
            IsProjectReady = false;
            LoadingProject = false;

            this.InitializeComponent();
            this.DataContext = this;
        }

        internal async Task<Project> LoadProject(string pPath)
        {
            try
            {
                StorageFolder pFolder = await StorageFolder.GetFolderFromPathAsync(pPath);

                string pFile = await ProjectHelper.TryGetProjectFile(pFolder);
                Project p = await Project.GetProjectFromJsonFile(pFile);
                return p;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }

        private void GoHome_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "Navigate", "NewProject"));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<NotificationMessage>(this, async (nm) =>
            {
                switch (nm.Target)
                {
                    case "LoadProject":
                        if (!LoadingProject)
                        {
                            LoadingProject = true;
                            PLoaderRing.Visibility = Visibility.Visible;
            
                            if (nm.Notification != null)
                            {
                                Debug.WriteLine(nm.Notification);
            
                                Project p = await LoadProject(nm.Notification);
            
                                if (p != null)
                                {
                                   
                                        Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "ChangeTitle", p.Name));
                                        IsProjectReady = true;
                                        LoadingProject = false;
                                        //InitialMessageText.Visibility = Visibility.Collapsed;
                                        PLoaderRing.Visibility = Visibility.Collapsed;
                                        ViewerFrame.Navigate(typeof(ProjectPreviewControl));
                                }
                                else
                                {
                                    IsProjectReady = false;
                                    LoadingProject = false;
                                    //InitialMessageText.Visibility = Visibility.Collapsed;
                                    PLoaderRing.Visibility = Visibility.Collapsed;
                                }
                            }
                            else
                            {
                                IsProjectReady = false;
                                LoadingProject = false;
                                //InitialMessageText.Visibility = Visibility.Collapsed;
                                PLoaderRing.Visibility = Visibility.Collapsed;
                            }
                        }
                        break;
                }
            });
        }

        private void ViewerFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}
