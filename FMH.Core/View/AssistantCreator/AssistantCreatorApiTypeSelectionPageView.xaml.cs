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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FMH.Core.View.AssistantCreator
{
    public partial class AssistantCreatorApiTypeSelectionPageView : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AssistantCreatorApiTypeSelectionPageView()
        {
            // Set control events
            this.Initialized += AssistantCreatorApiTypeSelectionPageView_Initialized;

            InitializeComponent();
        }

        #region Events
        private void AssistantCreatorApiTypeSelectionPageView_Initialized(object? sender, EventArgs e)
        {
            UITextTranslator.UpdateComponentsTranslations(MainGrid);
        }
        #endregion
    }
}
