using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3.Utils
{
    public class DirectoryUtils
    {
        public static bool CheckFolderIsForgeWorkspace(string path)
        {
            bool isForgeWorkspace = true;
            String[] filesToCheck = { @"build.gradle" };
            String[] foldersToCheck = { @"src", @"src\main", @"src\main\resources", @"src\main\java", @"src\main\resources\assets", @"src\main\resources\META-INF" };

            int id = 0;
            while (isForgeWorkspace && id < filesToCheck.Length)
            {
                isForgeWorkspace = File.Exists(Path.Combine(path, filesToCheck[id]));
                id++;
            }

            id = 0;
            while (isForgeWorkspace && id < foldersToCheck.Length)
            {
                isForgeWorkspace = Directory.Exists(Path.Combine(path, foldersToCheck[id]));
                id++;
            }

            return isForgeWorkspace;
        }
    }
}
