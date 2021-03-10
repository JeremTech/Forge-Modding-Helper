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

        /// <summary>
        /// Return a list of files in a directory and in his all sub-directories
        /// </summary>
        /// <param name="path">Targetted directory</param>
        public static List<string> DeepFileListing(string path)
        {
            List<string> fileList = new List<string>();

            try
            {
                // For each file in the directory
                foreach (string f in Directory.GetFiles(path))
                {
                    fileList.Add(f);
                }

                // For each sub directory
                foreach (string d in Directory.GetDirectories(path))
                {
                    fileList.AddRange(DeepFileListing(d));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return fileList;
        }
    }
}
