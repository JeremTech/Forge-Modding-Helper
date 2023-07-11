using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMH.Workspace.Data;

namespace FMH.Workspace.WorkspaceManager
{
    public interface IWorkspaceManager
    {
        /// <summary>
        /// Workspace properties
        /// </summary>
        WorkspaceProperties WorkspaceProperties { get; set; }

        /// <summary>
        /// Mod properties
        /// </summary>
        ModProperties ModProperties { get; set; }

        /// <summary>
        /// Mod versions history
        /// </summary>
        ModVersionsHistory ModVersionsHistory { get; set; }

        /// <summary>
        /// Assets properties
        /// </summary>
        AssetsProperties AssetsProperties { get; set; }

        /// <summary>
        /// Source code properties
        /// </summary>
        SourceCodeProperties SourceCodeProperties { get; set; }

        /// <summary>
        /// Read data from mod.toml file
        /// </summary>
        void ReadModToml();

        /// <summary>
        /// Read data from build.gradle file
        /// </summary>
        void ReadBuildGradle();

        /// <summary>
        /// Read data from gradle.properties file
        /// </summary>
        void ReadGradleProperties();

        /// <summary>
        /// Write mod.toml file
        /// </summary>
        void WriteModToml();

        /// <summary>
        /// Write build.gradle file
        /// </summary>
        void WriteBuildGradle();

        /// <summary>
        /// Write gradle.properties file
        /// </summary>
        void WriteGradleProperties();
    }
}
