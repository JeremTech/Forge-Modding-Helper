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
using FMH.Core.Files.Software;
using FMH.Core.Utils.UI;

namespace FMH.Core.UI.Common
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        // Fonction called when the window is initialized
        private void Window_Initialized(object sender, EventArgs e)
        {
            // Loadings translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
            this.Title = UITextTranslator.getTranslation("about.title");

            // Updating app infos
            version_label.Content = App.GetApplicationVersionString();
        }
    }
}
