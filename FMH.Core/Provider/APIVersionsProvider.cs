using FMH.Core.Model;
using FMH.Workspace.Data;
using McVersionsLib.Forge;
using McVersionsLib.NeoForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FMH.Core.Provider
{
    public static class APIVersionsProvider
    {
        /// <summary>
        /// Return all Minecraft Forge versions for all supported Minecraft version
        /// </summary>
        /// <returns>List of all Minecraft Forge versions</returns>
        public static List<APIVersionData> GetMinecraftForgeVersions()
        {
            var outputList = new List<APIVersionData>();

            foreach (var version in App.GetSupportedForgeMinecraftVersions())
            {
                try
                {
                    var forgeVersions = McForgeVersions.GetAllMinecraftForgeVersions(version);

                    foreach (var forgeVersion in forgeVersions)
                    {
                        outputList.Add(new APIVersionData()
                        {
                            ModAPIType = ModAPIType.Forge,
                            MinecraftVersion = version,
                            MinecraftVersionParsed = Version.Parse(version),
                            APIVersion = forgeVersion
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return outputList;
        }

        /// <summary>
        /// Return all Minecraft NeoForge versions for all supported Minecraft version
        /// </summary>
        /// <returns>List of all Minecraft NeoForge versions</returns>
        public static List<APIVersionData> GetMinecraftNeoForgeVersions()
        {
            var outputList = new List<APIVersionData>();

            foreach (var version in App.GetSupportedNeoForgeMinecraftVersions())
            {
                try
                {
                    var neoForgeVersions = McNeoForgeVersions.GetAllNeoForgeVersions(version);

                    foreach (var neoForgeVersion in neoForgeVersions)
                    {
                        outputList.Add(new APIVersionData()
                        {
                            ModAPIType = ModAPIType.NeoForge,
                            MinecraftVersion = version,
                            MinecraftVersionParsed = Version.Parse(version),
                            APIVersion = neoForgeVersion
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return outputList;
        }
    }
}
