using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Forge_Modding_Helper_3.Utils
{
    /// <summary>
    /// Class with some useful functions for UI
    /// </summary>
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
    }
}
