using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using UwpApp.Base;
using UwpApp.Models;

namespace UwpApp.ViewModel
{
    public interface IMainViewModel : INotifyPropertyChanged
    {
        ObservableCollection<GridEntry> Entries { get; set; }
        string FilterValue { get; set; }
        ObservableCollection<GridEntry> OriginalEntries { get; set; }
        GridEntry SelectedEntry { get; set; }
        string UserName { get; set; }

        void AddEntry();
        void DeleteEntry();
        void FilterEntries();
        Task LoadEntries();
        void Logout();
        void UpdateEntry(object sender, EventArgs e);
    }
}