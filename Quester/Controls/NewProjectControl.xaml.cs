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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quester.Controls
{
    public sealed partial class NewProjectControl : UserControl
    {
        public NewProjectControl()
        {
            this.InitializeComponent();
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
        }
    }
}
