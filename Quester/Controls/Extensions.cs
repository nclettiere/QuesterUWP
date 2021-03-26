using Quester.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Quester.Controls
{
    public class Extensions
    {
        public static readonly DependencyProperty OverWidthProperty =
        DependencyProperty.RegisterAttached("Project", typeof(Project), typeof(Extensions), new PropertyMetadata(default(Project)));

        public static void SetProject(UIElement element, Project value)
        {
            element.SetValue(OverWidthProperty, value);
        }

        public static Project GetProject(UIElement element)
        {
            return (Project)element.GetValue(OverWidthProperty);
        }
    }
}
