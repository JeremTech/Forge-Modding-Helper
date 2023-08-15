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
        /// <returns><c>true</c> if success, else <c>false</c></returns>
        bool ReadModToml();

        /// <summary>
        /// Read data from build.gradle file
        /// </summary>
        /// <returns><c>true</c> if success, else <c>false</c></returns>
        bool ReadBuildGradle();

        /// <summary>
        /// Read data from gradle.properties file
        /// </summary>
        /// <returns><c>true</c> if success, else <c>false</c></returns>
        bool ReadGradleProperties();

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

        /// <summary>
        /// Check if the workspace is valid for Forge Modding Helper
        /// </summary>
        /// <param name="supportedMinecraftVersions">Supported minecraft versions</param>
        /// <returns><c>true</c> if this is a valid workspace, else <c>false</c></returns>
        bool CheckWorkspaceValidity(List<string> supportedMinecraftVersions);
    }
}
