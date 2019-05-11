using Core.Data;
using System.Linq;
using UwpApp.Models;
using UwpApp.Startup;
using UwpApp.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Autofac;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly IMainViewModel viewModel;
        public MainPage()
        {
            this.InitializeComponent();
            viewModel = ConfigureServices.Container().Resolve<IMainViewModel>();

            Loaded += async (sender, arguments) => await viewModel.LoadEntries();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                viewModel.SelectedEntry = e.AddedItems.First() as GridEntry;
            }
        }

        private void AutoSuggestBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            var autoSuggestBox = sender as AutoSuggestBox;
            if (e.Key == Windows.System.VirtualKey.Enter
                || autoSuggestBox.Text.Length == 0)
            {
                viewModel.FilterEntries();
            }
        }
    }
}
