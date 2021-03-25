using Quester.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Input;
using Windows.Foundation.Metadata;
using Windows.Gaming.Input;
using Windows.System;
using Windows.System.Profile;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;

using Quester.Pages;
using Quester.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.Activation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Quester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        ProjectViewer ProjectViewerCached { get; set; }
        public static Frame RootFrame = null;
        private static Type CurrentFrameType = null;
        private static object LastNavSelectedItem = null;

        public VirtualKey ArrowKey;

        private bool _isGamePadConnected;
        private bool _isKeyboardConnected;
        private Microsoft.UI.Xaml.Controls.NavigationViewItem _allControlsMenuItem;
        private Microsoft.UI.Xaml.Controls.NavigationViewItem _newControlsMenuItem;

        public Microsoft.UI.Xaml.Controls.NavigationView NavigationView
        {
            get { return NavigationViewControl; }
        }

        public Action NavigationViewLoaded { get; set; }

        public DeviceType DeviceFamily { get; set; }

        public bool IsFocusSupported
        {
            get
            {
                return DeviceFamily == DeviceType.Xbox || _isGamePadConnected || _isKeyboardConnected;
            }
        }

        public PageHeader PageHeader
        {
            get
            {
                return UIHelper.GetDescendantsOfType<PageHeader>(NavigationViewControl).FirstOrDefault();
            }
        }

        public string SelectedProject { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.DataContext = new MainViewModel();
            ProjectViewerCached = new ProjectViewer();

            Messenger.Default.Register<NotificationMessage>(this, (nm) =>
            {
                switch(nm.Target)
                {
                    case "Navigate":
                        switch (nm.Notification)
                        {
                            case "NewProject":
                                NavigateTo(PageType.ProjectSelector);
                                break;
                        }
                        break;
                    case "ProjectSelected":
                        SelectedProject = nm.Notification;
                        NavigateTo(PageType.ProjectViewer);
                        Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "LoadProject", SelectedProject));
                        break;
                    case "ChangeTitle":
                        PageHeader.Title = nm.Notification;
                        break;
                }
            });

            // Workaround for VisualState issue that should be fixed
            // by https://github.com/microsoft/microsoft-ui-xaml/pull/2271
            NavigationViewControl.PaneDisplayMode = muxc.NavigationViewPaneDisplayMode.Left;

            //_navHelper = new RootFrameNavigationHelper(rootFrame, NavigationViewControl);

            SetDeviceFamily();
            Current = this;
            RootFrame = rootFrame;

            this.GotFocus += (object sender, RoutedEventArgs e) =>
            {
                // helpful for debugging focus problems w/ keyboard & gamepad
                if (FocusManager.GetFocusedElement() is FrameworkElement focus)
                {
                    Debug.WriteLine("got focus: " + focus.Name + " (" + focus.GetType().ToString() + ")");
                }
            };

            Gamepad.GamepadAdded += OnGamepadAdded;
            Gamepad.GamepadRemoved += OnGamepadRemoved;

            Window.Current.SetTitleBar(AppTitleBar);

            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, e) => UpdateAppTitle(s);
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;


            var viewTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            viewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            viewTitleBar.ButtonHoverBackgroundColor = ColorHelper.FromArgb(100, 0, 0, 0);
            viewTitleBar.ButtonPressedBackgroundColor = Colors.Transparent;
            viewTitleBar.ButtonForegroundColor = (Color)Resources["SystemBaseHighColor"];


            _isKeyboardConnected = Convert.ToBoolean(new KeyboardCapabilities().KeyboardPresent);



            // remove the solid-colored backgrounds behind the caption controls and system back button if we are in left mode
            // This is done when the app is loaded since before that the actual theme that is used is not "determined" yet
            Loaded += delegate (object sender, RoutedEventArgs e)
            {
                NavigationOrientationHelper.UpdateTitleBar(NavigationOrientationHelper.IsLeftMode);
                AddNavigationMenuItems();
                //NavigateTo(PageType.ProjectSelector);
            };

            NavigationViewControl.RegisterPropertyChangedCallback(muxc.NavigationView.PaneDisplayModeProperty, new DependencyPropertyChangedCallback(OnPaneDisplayModeChanged));
        }

        private void OnPaneDisplayModeChanged(DependencyObject sender, DependencyProperty dp)
        {
            var navigationView = sender as muxc.NavigationView;
            MainPage.Current.AppTitleBar.Visibility = navigationView.PaneDisplayMode == muxc.NavigationViewPaneDisplayMode.Top ? Visibility.Collapsed : Visibility.Visible;
        }

        void UpdateAppTitle(CoreApplicationViewTitleBar coreTitleBar)
        {
            //ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        public string GetAppTitleFromSystem()
        {
            return Windows.ApplicationModel.Package.Current.DisplayName;
        }

        public bool CheckNewControlSelected()
        {
            return _newControlsMenuItem.IsSelected;
        }

        public void EnsureNavigationSelection(string id)
        {
            foreach (object rawGroup in this.NavigationView.MenuItems)
            {
                if (rawGroup is muxc.NavigationViewItem group)
                {
                    foreach (object rawItem in group.MenuItems)
                    {
                        if (rawItem is muxc.NavigationViewItem item)
                        {
                            if ((string)item.Tag == id)
                            {
                                group.IsExpanded = true;
                                NavigationView.SelectedItem = item;
                                item.IsSelected = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void AddNavigationMenuItems()
        {
            // you can also add items in code behind
            NavigationView.MenuItems.Add(new muxc.NavigationViewItem() { Content = "Home", Icon = new SymbolIcon(Symbol.Home), Tag = "home" });
            NavigationView.MenuItems.Add(new muxc.NavigationViewItemSeparator());
            NavigationView.MenuItems.Add(new muxc.NavigationViewItem() { Content = "Project Viewer", Icon = new SymbolIcon(Symbol.Library), Tag = "projectviewer" });

            // set the initial SelectedItem 
            foreach (muxc.NavigationViewItemBase item in NavigationView.MenuItems)
            {
                if (item is muxc.NavigationViewItem && item.Tag.ToString() == "home")
                {
                    NavView_Navigate((muxc.NavigationViewItem)item);
                    NavigationView.SelectedItem = item;
                    break;
                }
            }
        }

        private void OnMenuFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            //switch ((sender as MenuFlyoutItem).Tag)
            //{
            //    case ControlInfoDataItem item:
            //        ProtocolActivationClipboardHelper.Copy(item);
            //        return;
            //    case ControlInfoDataGroup group:
            //        ProtocolActivationClipboardHelper.Copy(group);
            //        return;
            //}
        }

        private static IconElement GetIcon(string imagePath)
        {
            return imagePath.ToLowerInvariant().EndsWith(".png") ?
                        (IconElement)new BitmapIcon() { UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute), ShowAsMonochrome = false } :
                        (IconElement)new FontIcon()
                        {
                            FontFamily = new FontFamily("Segoe MDL2 Assets"),
                            Glyph = imagePath
                        };
        }

        private void SetDeviceFamily()
        {
            var familyName = AnalyticsInfo.VersionInfo.DeviceFamily;

            if (!Enum.TryParse(familyName.Replace("Windows.", string.Empty), out DeviceType parsedDeviceType))
            {
                parsedDeviceType = DeviceType.Other;
            }

            DeviceFamily = parsedDeviceType;
        }

        private void OnNewControlsMenuItemLoaded(object sender, RoutedEventArgs e)
        {
            if (IsFocusSupported && NavigationViewControl.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded)
            {
                controlsSearchBox.Focus(FocusState.Keyboard);
            }
        }

        private void OnGamepadRemoved(object sender, Gamepad e)
        {
            _isGamePadConnected = Gamepad.Gamepads.Any();
        }

        private void OnGamepadAdded(object sender, Gamepad e)
        {
            _isGamePadConnected = Gamepad.Gamepads.Any();
        }

        private void OnNavigationViewControlLoaded(object sender, RoutedEventArgs e)
        {
            // Delay necessary to ensure NavigationView visual state can match navigation
            Task.Delay(500).ContinueWith(_ => this.NavigationViewLoaded?.Invoke(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnNavigationViewItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            // Close any open teaching tips before navigation
            CloseTeachingTips();

            if (args.InvokedItemContainer.IsSelected)
            {
                // Clicked on an item that is already selected,
                // Avoid navigating to the same page again causing movement.
                return;
            }

            if (args.IsSettingsInvoked)
            {
                PageHeader.Title = "Settings";
                rootFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                // find NavigationViewItem with Content that equals InvokedItem
                var item = sender.MenuItems.OfType<muxc.NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item as muxc.NavigationViewItem);
            }
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Close any open teaching tips before navigation
            CloseTeachingTips();

            //if (e.SourcePageType == typeof(AllControlsPage) ||
            //    e.SourcePageType == typeof(NewControlsPage))
            //{
            //    NavigationViewControl.AlwaysShowHeader = false;
            //}
            //else
            //{
            //    NavigationViewControl.AlwaysShowHeader = true;
            //}

            switch (rootFrame.CurrentSourcePageType.Name)
            {
                case "ProjectSelector":
                    PageHeader.Title = "Select or Create Project";
                    break;
                case "NewProjectPage":
                    PageHeader.Title = "New Project";
                    break;
                case "SettingsPage":
                    PageHeader.Title = "Settings";
                    break;
            }


            //if (LastNavSelectedItem != null)
                //NavigationView.SelectedItem = LastNavSelectedItem;
        }
        private void CloseTeachingTips()
        {
            if (Current?.PageHeader != null)
            {
                Current.PageHeader.TeachingTip1.IsOpen = false;
                Current.PageHeader.TeachingTip3.IsOpen = false;
            }
        }

        private void OnControlsSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //var suggestions = new List<ControlInfoDataItem>();
                //
                //var querySplit = sender.Text.Split(" ");
                //foreach (var group in ControlInfoDataSource.Instance.Groups)
                //{
                //    var matchingItems = group.Items.Where(
                //        item =>
                //        {
                //            // Idea: check for every word entered (separated by space) if it is in the name, 
                //            // e.g. for query "split button" the only result should "SplitButton" since its the only query to contain "split" and "button"
                //            // If any of the sub tokens is not in the string, we ignore the item. So the search gets more precise with more words
                //            bool flag = true;
                //            foreach (string queryToken in querySplit)
                //            {
                //                // Check if token is not in string
                //                if (item.Title.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                //                {
                //                    // Token is not in string, so we ignore this item.
                //                    flag = false;
                //                }
                //            }
                //            return flag;
                //        });
                //    foreach (var item in matchingItems)
                //    {
                //        suggestions.Add(item);
                //    }
                //}
                //if (suggestions.Count > 0)
                //{
                //    controlsSearchBox.ItemsSource = suggestions.OrderByDescending(i => i.Title.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i.Title);
                //}
                //else
                //{
                //    controlsSearchBox.ItemsSource = new string[] { "No results found" };
                //}
            }
        }

        private void OnControlsSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //if (args.ChosenSuggestion != null && args.ChosenSuggestion is ControlInfoDataItem)
            //{
            //    var infoDataItem = args.ChosenSuggestion as ControlInfoDataItem;
            //    var itemId = infoDataItem.UniqueId;
            //    bool changedSelection = false;
            //    foreach (object rawItem in NavigationView.MenuItems)
            //    {
            //        // Check if we encountered the separator
            //        if (!(rawItem is muxc.NavigationViewItem))
            //        {
            //            // Skipping this item
            //            continue;
            //        }
            //
            //        var item = rawItem as muxc.NavigationViewItem;
            //
            //        // Check if we are this category
            //        if ((string)item.Content == infoDataItem.Title)
            //        {
            //            NavigationView.SelectedItem = item;
            //            changedSelection = true;
            //        }
            //        // We are not :(
            //        else
            //        {
            //            // Maybe one of our items is? ಠಿ_ಠ
            //            if (item.MenuItems.Count != 0)
            //            {
            //                foreach (muxc.NavigationViewItem child in item.MenuItems)
            //                {
            //                    if ((string)child.Content == infoDataItem.Title)
            //                    {
            //                        // We are the item corresponding to the selected one, update selection!
            //
            //                        // Deal with differences in displaymodes
            //                        if (NavigationView.PaneDisplayMode == muxc.NavigationViewPaneDisplayMode.Top)
            //                        {
            //                            // In Topmode, the child is not visible, so set parent as selected
            //                            // Everything else does not work unfortunately
            //                            NavigationView.SelectedItem = item;
            //                            item.StartBringIntoView();
            //                        }
            //                        else
            //                        {
            //                            // Expand so we animate
            //                            item.IsExpanded = true;
            //                            // Ensure parent is expanded so we actually show the selection indicator
            //                            NavigationView.UpdateLayout();
            //                            // Set selected item
            //                            NavigationView.SelectedItem = child;
            //                            child.StartBringIntoView();
            //                        }
            //                        // Set to true to also skip out of outer for loop
            //                        changedSelection = true;
            //                        // Break out of child iteration for loop
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //        // We updated selection, break here!
            //        if (changedSelection)
            //        {
            //            break;
            //        }
            //    }
            //    NavigationRootPage.RootFrame.Navigate(typeof(ItemPage), itemId);
            //}
            //else if (!string.IsNullOrEmpty(args.QueryText))
            //{
            //    NavigationRootPage.RootFrame.Navigate(typeof(SearchResultsPage), args.QueryText);
            //}
        }

        private void NavigationViewControl_PaneClosing(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewPaneClosingEventArgs args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavigationViewControl_PaneOpened(Microsoft.UI.Xaml.Controls.NavigationView sender, object args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            Thickness currMargin = AppTitleBar.Margin;
            if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness((sender.CompactPaneLength * 2), currMargin.Top, currMargin.Right, currMargin.Bottom);

            }
            else
            {
                AppTitleBar.Margin = new Thickness(sender.CompactPaneLength, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }

            UpdateAppTitleMargin(sender);
            UpdateHeaderMargin(sender);
        }

        private void UpdateAppTitleMargin(Microsoft.UI.Xaml.Controls.NavigationView sender)
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24;

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                AppTitle.TranslationTransition = new Vector3Transition();

                if ((sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Translation = new System.Numerics.Vector3(smallLeftIndent, 0, 0);
                }
                else
                {
                    AppTitle.Translation = new System.Numerics.Vector3(largeLeftIndent, 0, 0);
                }
            }
            else
            {
                Thickness currMargin = AppTitle.Margin;

                if ((sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Margin = new Thickness(smallLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
                else
                {
                    AppTitle.Margin = new Thickness(largeLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
            }
        }

        private void UpdateHeaderMargin(Microsoft.UI.Xaml.Controls.NavigationView sender)
        {
            if (PageHeader != null)
            {
                if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    Current.PageHeader.HeaderPadding = (Thickness)App.Current.Resources["PageHeaderMinimalPadding"];
                }
                else
                {
                    Current.PageHeader.HeaderPadding = (Thickness)App.Current.Resources["PageHeaderDefaultPadding"];
                }
            }
        }

        private void CtrlF_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            controlsSearchBox.Focus(FocusState.Programmatic);
        }

        private async void NavView_Navigate(muxc.NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    PageHeader.Title = "Select or Create Project";
                    rootFrame.Navigate(typeof(ProjectSelector));
                    break;

                case "projectviewer":
                    PageHeader.Title = "Project Viewer";
                    rootFrame.Navigate(typeof(ProjectViewer), ProjectViewerCached);
                    await ProjectViewerCached.LoadProject(SelectedProject);
                    //rootFrame.Navigate(typeof(ProjectViewer));
                    //Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "LoadProject", SelectedProject));
                    break;
            }
        }

        private void NavigationViewControl_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args)
        {
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
               

                //NavigationView.SelectedItem = HomeTab;
                //sender.Han
            }
        }

        public void NavigateTo(PageType pageType)
        {
            muxc.NavigationViewItem item;

            switch (pageType)
            {
                case PageType.ProjectSelector:
                    PageHeader.Title = "Select or Create Project";
                    item = NavigationView.MenuItems.OfType<muxc.NavigationViewItem>()
                        .First(x => (string)x.Tag == "home");
                    NavView_Navigate(item);
                    NavigationView.SelectedItem = item;
                    break;
                case PageType.ProjectViewer:
                    PageHeader.Title = "Project Viewer";
                    item = NavigationView.MenuItems.OfType<muxc.NavigationViewItem>()
                        .First(x => (string)x.Tag == "projectviewer");
                    NavView_Navigate(item);
                    Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "LoadProject", SelectedProject));
                    NavigationView.SelectedItem = item;
                    break;
            }
        }
    }


    public enum DeviceType
    {
        Desktop,
        Mobile,
        Other,
        Xbox
    }

    public enum PageType
    {
        Settings,
        ProjectSelector,
        ProjectViewer,
    }
}