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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quester.Controls
{
    public sealed partial class NewProjectControl : UserControl
    {
        public static readonly DependencyProperty ProjectNameProperty = DependencyProperty.Register("ProjectName", typeof(string), typeof(NewProjectControl), null);
        public static readonly DependencyProperty ProjectPathProperty = DependencyProperty.Register("ProjectPath", typeof(string), typeof(NewProjectControl), null);

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
        public delegate void ShipmentHandler(object sender, NewProjectArgs pArgs);

        public event ShipmentHandler OnFolderSubmit;

        public NewProjectData ProjectData
        {
            set
            {
                projectData = value;

                // We need to check whether a tracking number 
                // was assigned to the field.
                NewProjectArgs myArgs = new NewProjectArgs(projectData);

                // Tracking number is available, raise the event.
                OnFolderSubmit(this, myArgs);
                
            }
        }

        public NewProjectControl()
        {
            this.InitializeComponent();
<<<<<<< HEAD
            this.DataContext = this;
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
            ProjectPath = IOHelper.GetDefaultProjectDir();
=======
            this.DataContext = new ViewModels.NewProjectViewModel();
        }

        private void ProjectNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            string pName = ((ViewModels.NewProjectViewModel)this.DataContext).ProjectName;
            Debug.WriteLine("ProjectName = " + pName);
>>>>>>> ab39137e503e6132967c355f3d40b332f7232d95
        }
    }
}
