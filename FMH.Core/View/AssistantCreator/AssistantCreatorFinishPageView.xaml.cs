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
using FMH.Core.Utils.UI;

namespace FMH.Core.View.AssistantCreator
{
    /// <summary>
    /// Interaction logic for AssistantCreatorFinishPageView.xaml
    /// </summary>
    public partial class AssistantCreatorFinishPageView : UserControl
    {
        public AssistantCreatorFinishPageView()
        {
            // Set control events
            this.Initialized += AssistantCreatorFinishPageView_Initialized;

            InitializeComponent();
        }

        #region Events
        private void AssistantCreatorFinishPageView_Initialized(object? sender, EventArgs e)
        {
            UITextTranslator.UpdateComponentsTranslations(MainGrid);
        }
        #endregion
    }
}
