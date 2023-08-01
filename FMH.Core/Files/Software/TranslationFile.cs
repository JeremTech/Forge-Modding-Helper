using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Files.Software
{
    public class TranslationFile
    {
        public string name { get; set; }
        public Dictionary<String, String> entries { get; set; }
    }
}
