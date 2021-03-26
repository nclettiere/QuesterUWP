using GalaSoft.MvvmLight.Messaging;
using Quester.Data;
using Quester.Helper;
using Quester.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Quester.Pages
{
    /// <summary>
    /// Page that handles creation, deletion and selection of projects.
    /// </summary>
    public sealed partial class ProjectSelector : Page
    {
        public ProjectSelector()
        {
            this.InitializeComponent();
            DataContext = new ProjectSelectorModel();
        }

        private void NewProjectControl_OnFolderSubmit(object sender, Controls.NewProjectArgs pArgs)
        {
            NewProjectFlyout.ShowAt(CreateProjectButton);
            NewProjectCtrl.CustomPathEntered = true;
            NewProjectCtrl.ProjectPath = pArgs.Data.ProjectPath;
        }

        private void NewProjectFlyout_Closed(object sender, object e)
        {
            if(!NewProjectCtrl.CustomPathEntered)
                NewProjectCtrl.ClearAll();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NewProjectCtrl.parent = this;
        }

        internal void CloseNewProjectFlyout()
        {
            NewProjectFlyout.Hide();
        }

        internal void ReloadProjects()
        {
            ((ProjectSelectorModel)DataContext).RetrieveAll();
        }

        private async void OpenExplorerItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuFlyoutItem item = sender as MenuFlyoutItem;
                StorageFolder pFolder = await StorageFolder.GetFolderFromPathAsync((string)item.Tag);
                await Launcher.LaunchFolderAsync(pFolder);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // probably permissions issue
                // Todo: solve file management here
            }
        }

        private async void DeleteProjectItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuFlyoutItem item = sender as MenuFlyoutItem;
                StorageFolder pFolder = await StorageFolder.GetFolderFromPathAsync((string)item.Tag);

                DeleteConfirmation.PrimaryButtonClick += async (s, args) =>
                {
                    try
                    {
                        await pFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        ((ProjectSelectorModel)DataContext).RetrieveAll();
                    }
                    catch(FileNotFoundException FileNotFoundEx)
                    {
                        Debug.WriteLine(FileNotFoundEx.Message);
                    }
                };

                await DeleteConfirmation.ShowAsync(ContentDialogPlacement.Popup);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // probably permissions issue
                // Todo: solve file management here
            }
        }

        private async void OnProjectButtonClick(object sender, RoutedEventArgs e)
        {
            Button pButton = sender as Button;
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "ProjectSelected", (string)pButton.Tag));
        }
    }
}
