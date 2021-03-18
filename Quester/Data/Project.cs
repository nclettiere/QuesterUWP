using Quester.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quester.Data
{
    class Project
    {
        public string Name { get; set; }
        public string ProjectPath { get; set; }
        public string ProjectDescription { get; set; }
        public string QuesterVersion { get; set; }
        public IList<Engines> Engines { get; set; }
        public IList<string> EnginesPath { get; set; }

        public Project()
        {
            Engines = new List<Engines>();
            EnginesPath = new List<string>();
        }

        public Project(string name, string projectPath, string projectDescription, string questerVersion, List<Engines> engines, List<string> enginesPath)
        {
            Name = name;
            ProjectPath = projectPath;
            ProjectDescription = projectDescription;
            QuesterVersion = questerVersion;
            Engines = engines;
            EnginesPath = enginesPath;
        }

        internal void FillNewData(NewProjectData pData)
        {
            Name = pData.ProjectName;
            ProjectPath = pData.ProjectPath;
            ProjectDescription = pData.ProjectDesc;
            //QuesterVersion = <TODO>;
            //Engines = <TODO>;
            //EnginesPath = <TODO>;
        }

        internal void SaveProject()
        {
            if (String.IsNullOrEmpty(ProjectPath))
                return;

            ProjectHelper.SerializeProjectJSON(this);
        }
    }

    public enum Engines
    {
        Unreal,
        Unity,
        Godot
    }
}
