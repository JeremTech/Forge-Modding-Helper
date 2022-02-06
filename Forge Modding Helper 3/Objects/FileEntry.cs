using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3.Objects
{
    /// <summary>
    /// File representation for ListViews
    /// </summary>
    public class FileEntry
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public FontAwesomeIcon Icon { get; set; }

        public FileEntry(string filePath, FontAwesomeIcon fileIcon)
        {
            this.FilePath = filePath;
            this.FileName = Path.GetFileNameWithoutExtension(filePath);
            // For .png.mcmeta files
            if (this.FileName.EndsWith(".png")) this.FileName = Path.GetFileNameWithoutExtension(this.FileName);
            this.Icon = fileIcon;
        }
    }
}
