using FMH.Core.Model;
using FMH.Core.Provider;
using FMH.Core.Utils.UI;
using FMH.Core.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FMH.Core.View.AssistantCreator
{
    public partial class AssistantCreatorSecondPageView : Page, IComponentDisplayed
    {
        /// <summary>
        /// Available API versions list
        /// </summary>
        public ObservableCollection<APIVersionData> APIVersionsList { get; set; }

        private List<APIVersionData> _APIVersionsListCache {  get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commonDataContext">Common Assistant Creator's data context</param>
        public AssistantCreatorSecondPageView(AssistantCreatorViewModel commonDataContext)
        {
            InitializeComponent();
            this.DataContext = commonDataContext;

            APIVersionsList = new ObservableCollection<APIVersionData>();
            _APIVersionsListCache = new List<APIVersionData>();
        }

        /// <inheritdoc/>
        public async void OnComponentDisplayed()
        {
            var selectedItemCache = APIVersionsListView.SelectedItem;
            FilterTextBox.Text = string.Empty;

            LoadingSpinner.Visibility = Visibility.Visible;
            
            await Task.Run(() => LoadAPIVersionsList());

            APIVersionsListView.SelectedItem = APIVersionsList.FirstOrDefault(v => v.APIVersion == ((APIVersionData)selectedItemCache)?.APIVersion);

            LoadingSpinner.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Load available API versions list
        /// </summary>
        private void LoadAPIVersionsList()
        {
            // Filling cache
            _APIVersionsListCache.Clear();
            _APIVersionsListCache.AddRange(APIVersionsProvider.GetMinecraftForgeVersions());

            Dispatcher.Invoke(() =>
            {
                APIVersionsList.Clear();
                _APIVersionsListCache.ForEach(APIVersionsList.Add);
            });
        }

        /// <summary>
        /// APply a filter on the API versions list
        /// </summary>
        /// <param name="filter">Filter to apply</param>
        private void FilterAPIVersionsList(string filter)
        {
            Dispatcher.Invoke(() =>
            {
                APIVersionsList.Clear();

                if (string.IsNullOrEmpty(filter))
                {
                    _APIVersionsListCache.ForEach(APIVersionsList.Add);
                    return;
                }

                _APIVersionsListCache.Where(v => v.APIVersion.Contains(filter)).ToList().ForEach(APIVersionsList.Add);
            });
        }

        private async void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filteringText = (sender as TextBox)?.Text;
            var selectedItemCache = APIVersionsListView.SelectedItem;

            LoadingSpinner.Visibility = Visibility.Visible;

            await Task.Run(() => FilterAPIVersionsList(filteringText));

            APIVersionsListView.SelectedItem = APIVersionsList.FirstOrDefault(v => v.APIVersion == ((APIVersionData)selectedItemCache)?.APIVersion);

            LoadingSpinner.Visibility = Visibility.Collapsed;
        }
    }
}
