using FMH.Workspace.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Model
{
    /// <summary>
    /// New workspace data, used in the Assistant Creator
    /// </summary>
    public class NewWorkspace : ModelBase
    {
        private ModAPIType _modAPIType;

        /// <summary>
        /// Workspace's modding API
        /// </summary>
        public ModAPIType ModAPI 
        { 
            get 
            {  
                return _modAPIType; 
            } 
            set 
            { 
                _modAPIType = value;
                OnPropertyChanged(); 
            }
        }

        private APIVersionData _modAPIVersion;

        /// <summary>
        /// Workspace's mod API version
        /// </summary>
        public APIVersionData ModAPIVersion
        {
            get
            {
                return _modAPIVersion;
            }
            set
            {
                _modAPIVersion = value;
                OnPropertyChanged();
            }
        }
    }
}
