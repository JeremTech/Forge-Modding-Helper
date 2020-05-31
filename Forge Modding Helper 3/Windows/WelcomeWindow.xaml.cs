using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Utils;

namespace Forge_Modding_Helper_3.Windows
{
    /// <summary>
    /// Logique d'interaction pour WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Function called at the window initialization
        /// </summary>
        private void Window_Initialized(object sender, EventArgs e)
        {
            version_label.Content = AppInfos.GetApplicationVersionString();
        }

        #region Buttons
        /// <summary>
        /// Function called when the user click on the "new" button
        /// </summary>
        private void new_mod_button_Click(object sender, RoutedEventArgs e)
        {
            // Display the Assistant Creator
            AssistantCreator creator = new AssistantCreator();
            creator.ShowDialog();
        }
        #endregion
    }
}
