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

namespace Forge_Modding_Helper_3.Windows.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour LoadingDialog.xaml
    /// </summary>
    public partial class LoadingDialog : Window
    {
        public LoadingDialog(string firstStatusText)
        {
            InitializeComponent();

            // Update text
            UpdateStatusText(firstStatusText);
        }

        /// <summary>
        /// Set the status text
        /// </summary>
        /// <param name="statusText">Text to display as status</param>
        public void UpdateStatusText(string statusText) 
        { 
            this.StatusTextBlock.Text = statusText;
        }
    }
}
