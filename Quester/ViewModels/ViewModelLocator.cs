using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
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
}
