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

            // Set window related functions
            if (DataContext is WelcomeViewModel viewModel)
            {
                viewModel.CloseParentWindow = Close;
                viewModel.ReloadTranslationsAction = UpdateTranslations;
            }

            // Load translation
            UpdateTranslations();
        }

        internal void UpdateTranslations()
        {
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
            this.Title = UITextTranslator.GetTranslation("welcome.title");
        }
    }
}
