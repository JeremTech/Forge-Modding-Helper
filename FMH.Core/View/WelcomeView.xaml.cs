using FMH.Core.Utils.UI;
using FMH.Core.ViewModel;
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

namespace FMH.Core.View
{
    public partial class WelcomeView : Window
    {
        public WelcomeView()
        {
            InitializeComponent();

            // Define window's actions in the view model
            var viewModel = (WelcomeViewModel)this.DataContext;
            viewModel.CloseParentWindow = () => this.Close();

            // Load translations
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
            this.Title = UITextTranslator.GetTranslation("welcome.title");
        }
    }
}
