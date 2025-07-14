using McVersionsLib.Minecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FMH.Workspace.Provider
{
    /// <summary>
    /// Provider for mappings related data
    /// </summary>
    public static class MappingsProvider
    {
        /// <summary>
        /// Returns the Parchment mappings version for a given Minecraft version
        /// </summary>
        /// <param name="mcVersion">Targetted Minecraft version</param>
        /// <returns>Parchment mappings version to use for the targetted Minecraft version</returns>
        public static string GetParchmentMappingsVersion(string mcVersion)
        {
            var mappingsVersion = RetrieveParchmentMappingsVersionFromWeb(mcVersion).Result;
                
            if (!string.IsNullOrEmpty(mappingsVersion))
                return mappingsVersion;
            else
                throw new Exception($"Failed to retrieve Parchment mappings version for Minecraft version {mcVersion}");
        }

        /// <summary>
        /// Retrieves the Parchment mappings version from the web for a given Minecraft version
        /// </summary>
        /// <param name="mcVersion">Targetted Minecraft version</param>
        /// <returns>Parchment mappings version to use for the targetted Minecraft version, <c>null</c> if no found</returns>
        private async static Task<string?> RetrieveParchmentMappingsVersionFromWeb(string mcVersion)
        {
            var url = string.Format("https://ldtteam.jfrog.io/artifactory/parchmentmc-public/org/parchmentmc/data/parchment-{0}/maven-metadata.xml", GetParchmentMinecraftVersion(mcVersion));

            using var httpClient = new HttpClient();
            var xmlContent = await httpClient.GetStringAsync(url);

            var xdoc = XDocument.Parse(xmlContent);
            var releaseElement = xdoc.Descendants("release").FirstOrDefault();
            return releaseElement?.Value;
        }

        /// <summary>
        /// Return the Parchment minecraft version mappings for a specific Minecraft version 
        /// </summary>
        /// <param name="mcVersion">Targetted Minecraft version</param>
        /// <returns>Parchment minecraft version mappings for the targetted Minecraft version</returns>
        private static string GetParchmentMinecraftVersion(string mcVersion)
        {
            switch (mcVersion)
            {
                case "1.21.7":
                    return "1.21.6";
                default:
                    return mcVersion; // Default case, return the original Minecraft version
            }
        }
    }
}
