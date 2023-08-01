using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace FMH.Core.Utils.UI
{
    public class UIUtils
    {
        /// <summary>
        /// Get all control of type <c>T</c> of an element
        /// </summary>
        /// <typeparam name="T">Control type to enumerate</typeparam>
        /// <param name="depObj">Window or control to browse</param>
        /// <returns>Control list</returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Return list of all available themes files
        /// </summary>
        /// <returns>Return list of all available themes files</returns>
        public static List<string> GetAllAvailableThemesFiles()
        {
            List<String> themesList = new List<string>();
            string directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Themes");

            // Check if Themes directory exist
            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show("\"" + directoryPath + "\" not found.\nUnable to load themes.\n\n Default theme will be applied.", "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            List<string> themesFilesList = Directory.GetFiles(directoryPath).ToList();

            foreach (string filePath in themesFilesList)
            {
                themesList.Add(Path.GetFileNameWithoutExtension(filePath));
            }

            return themesList;
        }
    }
}
