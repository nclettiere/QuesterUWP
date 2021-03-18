using GalaSoft.MvvmLight.Messaging;
using Quester.Data;
using Quester.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ProjectSelector : Page
    {
       
        List<ProjectButtonData> ButtonsData;

        public ProjectSelector()
        {
            this.InitializeComponent();

            this.DataContext = new MainViewModel();

            ButtonsData = new List<ProjectButtonData>();
            ButtonsData.Add(new ProjectButtonData("Test1", "Tooltip1", @"C:\Users\Percebe64\source\repos\QuesterUWP\Quester"));
            ButtonsData.Add(new ProjectButtonData("Test2", "Tooltip2", @"C:\Users\Percebe64\source\repos\QuesterUWP\Quester"));
            ButtonsData.Add(new ProjectButtonData("Test3", "Tooltip3", @"C:\Users\Percebe64\source\repos\QuesterUWP\Quester"));
        }

        private void BasicGridView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "Navigate", "NewProject"));
        }

        private void NewProjectControl_OnFolderSubmit(object sender, Controls.NewProjectArgs pArgs)
        {
            Debug.WriteLine("Project Data: " + pArgs.Data.ToString());
            NewProjectFlyout.ShowAt(CreateProjectButton);
            NewProjectCtrl.CustomPathEntered = true;
            NewProjectCtrl.ProjectPath = pArgs.Data.ProjectPath;
        }

        private void NewProjectFlyout_Closed(object sender, object e)
        {
            if(!NewProjectCtrl.CustomPathEntered)
                NewProjectCtrl.ClearAll();
        }
    }
}
