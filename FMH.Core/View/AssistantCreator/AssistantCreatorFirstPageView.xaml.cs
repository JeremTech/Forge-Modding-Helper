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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FMH.Core.View.AssistantCreator
{
    /// <summary>
    /// Logique d'interaction pour AssistantCreatorFirstPageView.xaml
    /// </summary>
    public partial class AssistantCreatorFirstPageView : Page
    {
        public AssistantCreatorFirstPageView(AssistantCreatorViewModel commonContext)
        {
            InitializeComponent();
            this.DataContext = commonContext;
        }

        public void SetDataContext(AssistantCreatorViewModel commonContext)
        {
            this.DataContext = commonContext;
        }
    }
}
