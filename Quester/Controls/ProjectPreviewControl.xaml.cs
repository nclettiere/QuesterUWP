using GalaSoft.MvvmLight.Messaging;
using Quester.Data;
using System;
using System.Collections.Generic;
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

namespace Quester.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectPreviewControl : Page
    {
        public Project PreviewProject { get; set; }
        public bool IsProjectReady { get; set; }

        public ProjectPreviewControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
            PreviewProject = null;
            IsProjectReady = false;
        }


        internal void ProjectNeedsUpdate()
        {
            PreviewProject = Extensions.GetProject(this);
            if (PreviewProject != null)
            {
                IsProjectReady = true;
                InitialMessageText.Visibility = Visibility.Collapsed;
                ProjectNameText.Text = PreviewProject.Name;
                return;
            }
            InitialMessageText.Visibility = Visibility.Visible;
        }

        private void GoHome_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "Navigate", "NewProject"));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
