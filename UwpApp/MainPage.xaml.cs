using Core.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UwpApp.Models;
using UwpApp.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainViewModel viewModel;
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (viewModel == null)
            {
                viewModel = new MainViewModel((MbaApiClient)e.Parameter);
            }
            await viewModel.LoadEntries();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                viewModel.SelectedEntry = e.AddedItems.First() as GridEntry;
            }
        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Key == Windows.System.VirtualKey.Enter
                || textBox.Text.Length == 0)
            {
                viewModel.FilterValue = textBox.Text;
            }
        }
    }
}
