using FMH.Core.Model;
using McVersionsLib.Forge;
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

            foreach (var version in App.GetSupportedMinecraftVersions())
            {
                try
                {
                    var forgeVersions = McForgeVersions.GetAllMinecraftForgeVersions(version);

                    foreach (var forgeVersion in forgeVersions)
                    {
                        outputList.Add(new APIVersionData()
                        {
                            ModAPIType = Workspace.Data.ModAPIType.Forge,
                            MinecraftVersion = version,
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
    }
}
