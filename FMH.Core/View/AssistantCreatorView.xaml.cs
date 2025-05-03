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
using FMH.Core.ViewModel;

namespace FMH.Core.View
{
    public partial class AssistantCreatorView : Window
    {
        public AssistantCreatorView()
        {
            InitializeComponent();

            // Set window related functions
            if (DataContext is AssistantCreatorViewModel viewModel)
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
    }
}
