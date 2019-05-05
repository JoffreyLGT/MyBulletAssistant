using Core.Data;
using Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UwpApp.Base;
using UwpApp.Models;
using UwpApp.Startup;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Autofac;

namespace UwpApp.ViewModel
{
    public class MainViewModel : Observable, IMainViewModel
    {
        private readonly IDataProvider dataProvider;
        private ObservableCollection<GridEntry> entries;
        private ObservableCollection<GridEntry> originalEntries;
        private GridEntry selectedEntry;
        private string filterValue;
        private string userName;

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                OnPropertyChanged();
            }
        }



        public string FilterValue
        {
            get => filterValue;
            set
            {
                filterValue = value;
                OnPropertyChanged();
            }
        }

        public GridEntry SelectedEntry
        {
            get => selectedEntry;
            set
            {
                this.selectedEntry = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<GridEntry> Entries
        {
            get => entries;
            set
            {
                this.entries = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GridEntry> OriginalEntries
        {
            get => originalEntries;
            set
            {
                this.originalEntries = value;
                Entries = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            this.dataProvider = ConfigureServices.Container().Resolve<IDataProvider>();
        }

        public async Task LoadEntries()
        {
            (UserModel user, string error) = await dataProvider.GetUser(true);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.WriteLine("An error occured while fetching the user's entries.");
                return;
            }
            ObservableCollection<GridEntry> entries = new ObservableCollection<GridEntry>();
            foreach (var entry in user.Entries)
            {
                GridEntry gridEntry = new GridEntry(entry);
                gridEntry.OnChange += UpdateEntry;
                entries.Add(gridEntry);
            }
            OriginalEntries = entries;
            UserName = user.Email;
        }

        public async void UpdateEntry(object sender, EventArgs e)
        {
            var editedEntry = ((GridEntry)sender).ToEntryModel();
            (_, string error) = await dataProvider.UpdateEntry(editedEntry.Id, editedEntry);
            if (string.IsNullOrEmpty(error))
            {
                return;
            }
            Debug.WriteLine($"Error while trying to update the entry: {error}");
            var dialog = new MessageDialog("Something went wrong while updating the entry.");
            await dialog.ShowAsync();
        }

        public async void AddEntry()
        {
            (var entry, var error) = await dataProvider.CreateEntry(new EntryModel { Journal = "New", Pages = "New", Title = "New" });
            if (!string.IsNullOrEmpty(error))
            {
                Debug.WriteLine($"Error while trying to update the entry: {error}.");
                var dialog = new MessageDialog("Something went wrong while updating the entry.");
                await dialog.ShowAsync();
                return;
            }
            OriginalEntries.Add(new GridEntry(entry));
            Entries.Add(new GridEntry(entry));
        }

        public async void DeleteEntry()
        {
            if (selectedEntry == null)
            {
                return;
            }
            (var isDeleted, var error) = await dataProvider.DeleteEntry(selectedEntry.Id);
            if (!isDeleted)
            {
                Debug.WriteLine($"Error while trying to delete the entry: {error}");
                var dialog = new MessageDialog("Something went wrong while deleting the entry.");
                await dialog.ShowAsync();
                return;
            }
            OriginalEntries.Remove(selectedEntry);
            Entries.Remove(selectedEntry);
        }

        public void FilterEntries()
        {
            if (string.IsNullOrWhiteSpace(FilterValue))
            {
                Entries = OriginalEntries;
                return;
            }
            var rqtFilter = from entry in OriginalEntries
                            where entry.ToString().ToUpperInvariant().Contains(FilterValue.ToUpperInvariant())
                            select entry;
            Entries = new ObservableCollection<GridEntry>(rqtFilter);
        }

        public void Logout()
        {
            var vault = new PasswordVault();
            vault.Remove(vault.FindAllByResource("My Bullet Assistant")[0]);
            Environment.Exit(0);
        }
    }
}
