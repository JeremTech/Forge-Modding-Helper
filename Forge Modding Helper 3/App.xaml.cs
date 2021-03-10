using Forge_Modding_Helper_3.Objects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Forge_Modding_Helper_3
{
    /// <summary>
    /// Global app values
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Current project object
        /// </summary>
        public static Project CurrentProjectData { get; set; }
    }
}
