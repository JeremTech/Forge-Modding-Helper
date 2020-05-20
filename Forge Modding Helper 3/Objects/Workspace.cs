﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Forge_Modding_Helper_3.Objects
{
    public class Workspace
    {
        public string mod_name { get; set; }
        public string description { get; set; }
        public string minecraft_version { get; set; }
        public string path { get; set; }

        public Workspace(string modName, string minecraftVersion, string path, string description)
        {
            this.mod_name = modName;
            this.minecraft_version = minecraftVersion;
            this.path = path;
            this.description = description;
        }

        public string GetModName()
        {
            return this.mod_name;
        }

        public string GetMinecraftVersion()
        {
            return this.minecraft_version;
        }

        public string GetPath()
        {
            return this.path;
        }

        public string GetDescription()
        {
            return this.description;
        }
    }
}
