using Core.Data;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UwpApp.Base;
using UwpApp.Models;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace UwpApp.ViewModel
{
    public class MainViewModel : Observable
    {
        private readonly MbaApiClient client;
        private ObservableCollection<GridEntry> entries;
        private ObservableCollection<GridEntry> originalEntries;
        private GridEntry selectedEntry;
        private string filterValue;

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

        public MainViewModel(MbaApiClient client)
        {
            this.client = client;
        }

        public async Task LoadEntries()
        {
            (EntryModel[] dbEntries, string error) = await client.GetEntries();
            if (!string.IsNullOrEmpty(error))
            {
                Debug.WriteLine("An error occured while fetching the user's entries.");
                return;
            }
            ObservableCollection<GridEntry> entries = new ObservableCollection<GridEntry>();
            foreach (var entry in dbEntries)
            {
                GridEntry gridEntry = new GridEntry(entry);
                gridEntry.OnChange += UpdateEntry;
                entries.Add(gridEntry);
            }
            OriginalEntries = entries;
        }

        public async void UpdateEntry(object sender, EventArgs e)
        {
            var editedEntry = ((GridEntry)sender).ToEntryModel();
            (_, string error) = await client.UpdateEntry(editedEntry.Id, editedEntry);
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
            (var entry, var error) = await client.CreateEntry(new EntryModel { Journal = "New", Pages = "New", Title = "New" });
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
            (var isDeleted, var error) = await client.DeleteEntry(selectedEntry.Id);
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
    }
}
