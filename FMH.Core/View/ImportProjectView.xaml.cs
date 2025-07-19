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
    /// <summary>
    /// Interaction logic for ImportProjectView.xaml
    /// </summary>
    public partial class ImportProjectView : Window
    {
        public ImportProjectView()
        {
            InitializeComponent();

            // Load translation
            UpdateTranslations();

            // Set window related functions
            if (DataContext is ImportProjectViewModel viewModel)
            {
                viewModel.CloseWindowAction = Close;
                viewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(viewModel.DialogResult))
                    {
                        DialogResult = viewModel.DialogResult;
                    }
                };
            }
        }

        internal void UpdateTranslations()
        {
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
            this.Title = UITextTranslator.GetTranslation("project_importation.title");
        }
    }
}
