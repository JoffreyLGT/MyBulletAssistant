using Core.Data;
using Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using UwpApp.Base;
using UwpApp.Models;
using Windows.UI.Popups;

namespace UwpApp.ViewModel
{
    public class MainViewModel : Observable
    {
        private readonly MbaApiClient client;
        private ObservableCollection<GridEntry> entries;
        private GridEntry selectedEntry;

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
            Entries = entries;
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
            Entries.Remove(selectedEntry);
        }
    }
}
