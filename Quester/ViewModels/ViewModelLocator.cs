using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quester.ViewModels
{
    class ViewModelLocator
    {
        public MainViewModel MainPage
        {
            get { return new MainViewModel(); }
        }
    }

    public class MainViewModel
    {
        public MainViewModel() { }

        public void Close()
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "CloseWindowsBoundToMe"));
        }
    }

    public class NewProjectViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // props
        private string projectname;
        public string ProjectName
        {
            get { return projectname; }
            set { SetField(ref projectname, value, "Name"); }
        }
        public NewProjectViewModel() { }
    }
}
