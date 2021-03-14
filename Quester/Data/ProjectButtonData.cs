using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quester.Data
{
    class ProjectButtonData
    {
        public string ProjectName { get; set; }
        public string ProjectTooltip { get; set; }
        public string ProjectPath { get; set; }

        public ProjectButtonData()
        {
        }

        public ProjectButtonData(string pName, string pTooltip, string pPath)
        {
            ProjectName = pName;
            ProjectTooltip = pTooltip;
            ProjectPath = pPath;
        }
    }
}
