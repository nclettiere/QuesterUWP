using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Quester.Controls
{
    public struct NewProjectData
    {
        public NewProjectData(string projectName, string projectPath)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
        }

        public string ProjectName { get; }
        public string ProjectPath { get; }

        public override string ToString() => $"projectName({ProjectName}), projectPath({ProjectPath})";
    }

    public class NewProjectArgs : EventArgs
    {
        private NewProjectData data;

        public NewProjectArgs(NewProjectData data)
        {
            this.data = data;
        }

        // This is a straightforward implementation for 
        // declaring a public field
        public NewProjectData Data
        {
            get
            {
                return data;
            }
        }
    }

    public class GameService : DependencyObject
    {
        public static readonly DependencyProperty IsMovableProperty =
        DependencyProperty.RegisterAttached(
          "IsMovable",
          typeof(Boolean),
          typeof(GameService),
          new PropertyMetadata(false)
        );
        public static void SetIsMovable(UIElement element, Boolean value)
        {
            element.SetValue(IsMovableProperty, value);
        }
        public static Boolean GetIsMovable(UIElement element)
        {
            return (Boolean)element.GetValue(IsMovableProperty);
        }
    }
}
