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

namespace Forge_Modding_Helper_3
{
    /// <summary>
    /// Logique d'interaction pour WorkspaceManager.xaml
    /// </summary>
    public partial class WorkspaceManager : Window
    {
        public WorkspaceManager()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void execute_button_genIJRuns_Click(object sender, RoutedEventArgs e)
        {
            GradleTasks tasks = new GradleTasks(this.textBox);
            tasks.genIntelljRuns();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            MessageBox.Show("Nous avons détecté un espace de travail Forge déjà configuré. Le gestionnaire d'espace de travail n'étant pas encore prêt, il n'y a rien de plus pour le moment !" + Environment.NewLine + Environment.NewLine + "L'application va se fermer.", "Informations", MessageBoxButton.OK, MessageBoxImage.Information);
            Environment.Exit(0);
        }
    }
}
