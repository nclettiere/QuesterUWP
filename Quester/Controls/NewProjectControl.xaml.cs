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

using Quester.Helper;
using System.Text.RegularExpressions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quester.Controls
{
    public sealed partial class NewProjectControl : UserControl
    {
        public static readonly DependencyProperty ProjectNameProperty = DependencyProperty.Register("ProjectName", typeof(string), typeof(NewProjectControl), null);
        public static readonly DependencyProperty ProjectPathProperty = DependencyProperty.Register("ProjectPath", typeof(string), typeof(NewProjectControl), null);

        private bool customPathEntered;
        private bool loadingStateEnabled;

        public bool CustomPathEntered
        {
            get { return customPathEntered; }
            set
            {
                customPathEntered = value;
            }
        }

        public bool LoadingStateEnabled
        {
            get { return loadingStateEnabled; }
            set
            {
                loadingStateEnabled = value;

                CreateProjectButton.IsEnabled = !value;
                ProjectNameTextBox.IsEnabled = !value;
                ProjectPathTextbox.IsEnabled = !value;
                ProjectDescRTB.IsEnabled = !value;
                SelectPathButton.IsEnabled = !value;
                ProjectProgressBar.IsActive = value;
                LoadingPhase.Visibility = !value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string ProjectName
        {
            get { return (string)GetValue(ProjectNameProperty); }
            set {
                SetValue(ProjectNameProperty, value);
            }
        }

        public string ProjectPath
        {
            get { return (string)GetValue(ProjectPathProperty); }
            set {
                string path = IOHelper.FormatProjectPath(ProjectNameTextBox.Text, value);
                SetValue(ProjectPathProperty, path);
            }
        }

        private NewProjectData projectData;

        // The delegate procedure we are assigning to our object
        public delegate void NewProjectHandler(object sender, NewProjectArgs pArgs);

        public event NewProjectHandler OnFolderSubmit;

        public NewProjectData ProjectData
        {
            set
            {
                projectData = value;
                NewProjectArgs pArgs = new NewProjectArgs(projectData);
                OnFolderSubmit(this, pArgs);
            }
        }

        internal void ClearAll()
        {
            ProjectName = String.Empty;
            ProjectPath = String.Empty;
            ProjectDescRTB.Document.SetText(Windows.UI.Text.TextSetOptions.ApplyRtfDocumentDefaults, String.Empty);
            CreateProjectButton.IsEnabled = false;
            customPathEntered = false;

            LoadingStateEnabled = false;
        }

        public NewProjectControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        // Checks if project creation is available 
        private bool CheckCreateProject()
        {
            bool validTexts =
            (String.IsNullOrWhiteSpace(ProjectNameTextBox.Text) || 
                String.IsNullOrWhiteSpace(ProjectPathTextbox.Text)) ? false : true;

            bool projectAvailable = IOHelper.IsProjectAvailable(ProjectPathTextbox.Text);

            CreateProjectButton.IsEnabled = validTexts && projectAvailable;

            return (validTexts && projectAvailable);
        }

        private async void SelectPathButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FolderPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                Debug.WriteLine("Picked folder: " + folder.Path);

                ProjectData = new NewProjectData(ProjectNameTextBox.Text, folder.Path);
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }
        }

        private void ProjectNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(ProjectNameTextBox.Text)) // This will prevent exception when textbox is empty   
            {
                if (!Regex.IsMatch(ProjectNameTextBox.Text, "^[a-zA-Z]+$"))
                {
                    ProjectNameTextBox.Text = ProjectNameTextBox.Text.Remove(ProjectNameTextBox.Text.Length - 1);
                    ProjectNameTextBox.Focus(FocusState.Keyboard);
                    ProjectNameTextBox.Select(ProjectNameTextBox.Text.Length, 0);
                }
            }else
            {
                CustomPathEntered = false;
            }

            if (!CustomPathEntered)
                ProjectPath = IOHelper.GetDefaultProjectDir();
            CheckCreateProject();
        }

        private void ProjectPathTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckCreateProject();
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if(CheckCreateProject())
            {
                LoadingStateEnabled = true;

                string desc = String.Empty;

                ProjectDescRTB.TextDocument.GetText(Windows.UI.Text.TextGetOptions.None, out desc);

                NewProjectData pData = new NewProjectData(ProjectName, ProjectPath, desc);

                ProjectHelper.CreateNewProject(this, pData);
            }
        }
    }
}
