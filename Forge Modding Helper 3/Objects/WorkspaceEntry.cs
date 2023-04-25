using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Forge_Modding_Helper_3.Objects
{
    // This class represent a workspace
    public class WorkspaceEntry
    {
        public string path { get; set; }
        public DateTime lastUpdated { get; set; }

        // Constructor
        public WorkspaceEntry(string _path, DateTime _lastUpdated)
        {
            this.path = _path;
            this.lastUpdated = _lastUpdated;
        }
    }
}
