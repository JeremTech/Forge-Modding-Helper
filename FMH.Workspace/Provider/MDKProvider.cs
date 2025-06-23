using McVersionsLib.Forge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Workspace.Provider
{
    /// <summary>
    /// Provider for Minecraft Development Kit (MDK) related functionalities.
    /// </summary>
    public static class MDKProvider
    {
        /// <summary>
        /// Provides a link to download the Minecraft Forge MDK for a specified API version
        /// </summary>
        /// <param name="forgeAPIVersion">Targetted Minecraft Forge API version</param>
        /// <returns>Minecraft Forge MDK download link for the targetted Minecraft Forge version</returns>
        public static string GetMinecraftForgeMDKLink(string forgeAPIVersion)
        {
            return McForgeUtils.BuildMinecraftForgeMDKDownloadLink(forgeAPIVersion);
        }

        /// <summary>
        /// Provides a link to download the NeoForge MDK for a specified Minecraft version
        /// </summary>
        /// <param name="mcVersion">Targetted Minecraft version</param>
        /// <returns>NeoForge MDK download link for the targetted Minecraft version</returns>
        public static string GetNeoForgeMDKLink(string mcVersion)
        {
            return string.Format("https://github.com/NeoForgeMDKs/MDK-{0}-NeoGradle/archive/refs/heads/main.zip", mcVersion);
        }
    }
}
