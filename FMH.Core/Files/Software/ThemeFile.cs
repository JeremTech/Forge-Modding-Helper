using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Files.Software
{
    public class ThemeFile
    {
        public string name { get; set; }
        public string PrimaryBackgroundColor { get; set; }
        public string SecondaryBackgroundColor { get; set; }
        public string InputsBackgroundColor { get; set; }
        public string FontColorPrimary { get; set; }
        public string FontColorSecondary { get; set; }
        public string BorderColor { get; set; }
        public string BorderErrorColor { get; set; }
        public string StandardButtonColor { get; set; }
        public string StandardButtonHoveredColor { get; set; }
    }
}
