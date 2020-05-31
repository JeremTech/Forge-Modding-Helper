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
        /// <summary>
        /// Check if a directory contains a Forge workspace or not
        /// </summary>
        /// <param name="path">Directory to check</param>
        /// <returns></returns>
        public static bool CheckFolderIsForgeWorkspace(string path)
        {
            // Result variable
            bool isForgeWorkspace = true;
            // Files to check
            String[] filesToCheck = { @"build.gradle" };
            // Folders to check
            String[] foldersToCheck = { @"src", @"src\main", @"src\main\resources", @"src\main\java", @"src\main\resources\assets", @"src\main\resources\META-INF" };

            // Files checking
            int id = 0;
            while (isForgeWorkspace && id < filesToCheck.Length)
            {
                isForgeWorkspace = File.Exists(Path.Combine(path, filesToCheck[id]));
                id++;
            }

            // Folders checking
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
