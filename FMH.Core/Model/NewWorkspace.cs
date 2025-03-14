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
        #region Mod API
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
        #endregion

        #region Mod API version
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
        #endregion

        #region Mod name
        private string _modName;

        /// <summary>
        /// Workspace's mod name
        /// </summary>
        public string ModName
        {
            get
            {
                return _modName;
            }
            set
            {
                _modName = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod authors
        private string _modAuthors;

        /// <summary>
        /// Workspace's mod authors
        /// </summary>
        public string ModAuthors
        {
            get
            {
                return _modAuthors;
            }
            set
            {
                _modAuthors = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod credits
        private string _modCredits;

        /// <summary>
        /// Workspace's mod credits
        /// </summary>
        public string ModCredits
        {
            get
            {
                return _modCredits;
            }
            set
            {
                _modCredits = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod license
        private string _modLicense;

        /// <summary>
        /// Workspace's mod license
        /// </summary>
        public string ModLicense
        {
            get
            {
                return _modLicense;
            }
            set
            {
                _modLicense = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod description
        private string _modDescription;

        /// <summary>
        /// Workspace's mod description
        /// </summary>
        public string ModDescription
        {
            get
            {
                return _modDescription;
            }
            set
            {
                _modDescription = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod id
        private string _modId;

        /// <summary>
        /// Workspace's mod id
        /// </summary>
        public string ModId
        {
            get
            {
                return _modId;
            }
            set
            {
                _modId = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod group
        private string _modGroup;

        /// <summary>
        /// Workspace's mod group
        /// </summary>
        public string ModGroup
        {
            get
            {
                return _modGroup;
            }
            set
            {
                _modGroup = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod version
        private string _modVersion;

        /// <summary>
        /// Workspace's mod version
        /// </summary>
        public string ModVersion
        {
            get
            {
                return _modVersion;
            }
            set
            {
                _modVersion = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod update json file link
        private string _updateJsonFileLink;

        /// <summary>
        /// Workspace's mod update JSON file link
        /// </summary>
        public string UpdateJsonFileLink
        {
            get
            {
                return _updateJsonFileLink;
            }
            set
            {
                _updateJsonFileLink = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod website
        private string _modWebsite;

        /// <summary>
        /// Workspace's mod website
        /// </summary>
        public string ModWebsite
        {
            get
            {
                return _modWebsite;
            }
            set
            {
                _modWebsite = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod issue tracker
        private string _modIssueTracker;

        /// <summary>
        /// Workspace's mod website
        /// </summary>
        public string ModIssueTracker
        {
            get
            {
                return _modIssueTracker;
            }
            set
            {
                _modIssueTracker = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Mod logo
        private string _modLogoSourcePath;

        /// <summary>
        /// Workspace's mod logo source path
        /// </summary>
        public string ModLogoSourcePath
        {
            get
            {
                return _modLogoSourcePath;
            }
            set
            {
                _modLogoSourcePath = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
