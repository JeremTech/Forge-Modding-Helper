using FMH.Core.Model;
using FMH.Core.Provider;
using FMH.Core.Utils.UI;
using FMH.Core.ViewModel;
using FMH.Workspace.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FMH.Core.View.AssistantCreator
{
    public partial class AssistantCreatorApiVersionSelectionPageView : UserControl, IComponentDisplayed, IComponentValidated
    {
        /// <summary>
        /// Available API versions list
        /// </summary>
        public ObservableCollection<APIVersionData> APIVersionsList { get; set; }

        private List<APIVersionData> _APIVersionsListCache {  get; set; }

        private AssistantCreatorViewModel? _viewModelDataContext;

        /// <summary>
        /// Constructor
        /// </summary>
        public AssistantCreatorApiVersionSelectionPageView()
        {
            // Set control events
            this.Initialized += AssistantCreatorApiVersionSelectionPageView_Initialized;

            InitializeComponent();

            // Initialize properties
            APIVersionsList = new ObservableCollection<APIVersionData>();
            _APIVersionsListCache = new List<APIVersionData>();
        }

        #region Events
        private void AssistantCreatorApiVersionSelectionPageView_Initialized(object? sender, System.EventArgs e)
        {
            UITextTranslator.UpdateComponentsTranslations(MainGrid);
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
        #endregion

        #region Interfaces implementations
        /// <inheritdoc/>
        public async void OnComponentDisplayed(params object[] args)
        {
            LoadingSpinner.Visibility = Visibility.Visible;

            // Retrieve parent DataContext from arguments
            if(args.Any() && args[0] is AssistantCreatorViewModel)
                _viewModelDataContext = args[0] as AssistantCreatorViewModel;

            // Load API versions list if not already loaded or if the user has changed the mod API type
            if (!_APIVersionsListCache.Any() || _APIVersionsListCache.FirstOrDefault()?.ModAPIType != _viewModelDataContext?.NewWorkspaceData.ModAPI)
            {
                await Task.Run(() => LoadAPIVersionsList());
            }

            LoadingSpinner.Visibility = Visibility.Collapsed;
        }

        /// <inheritdoc/>
        public bool ValidateData()
        {
            if (APIVersionsListView.SelectedItem == null)
            {
                MessageBox.Show("Please select an API version", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
        #endregion

        #region Versions list functions
        /// <summary>
        /// Load available API versions list
        /// </summary>
        private void LoadAPIVersionsList()
        {
            // Filling cache
            _APIVersionsListCache.Clear();
            switch (_viewModelDataContext?.NewWorkspaceData.ModAPI)
            {
                case ModAPIType.Forge:
                    _APIVersionsListCache.AddRange(APIVersionsProvider.GetMinecraftForgeVersions());
                    break;
                default:
                    break;
            }

            Dispatcher.Invoke(() =>
            {
                APIVersionsList.Clear();
                _APIVersionsListCache.ForEach(APIVersionsList.Add);
            });
        }

        /// <summary>
        /// Apply a filter on the API versions list
        /// </summary>
        /// <param name="filter">Filter to apply</param>
        private void FilterAPIVersionsList(string? filter)
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
        #endregion
    }
}
