using FMH.Workspace.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Model
{
    public class APIVersionData
    {
        /// <summary>
        /// Mod API type
        /// </summary>
        public ModAPIType ModAPIType { get; set; }

        /// <summary>
        /// Minecraft version
        /// </summary>
        public string MinecraftVersion { get; set; }

        /// <summary>
        /// Minecraft version parsed<br/>
        /// Used for comparisons
        /// </summary>
        public Version MinecraftVersionParsed { get; set; }

        /// <summary>
        /// API version
        /// </summary>
        public string APIVersion { get; set; }
    }
}
