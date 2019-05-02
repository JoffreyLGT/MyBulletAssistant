using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpApp.Base;

namespace UwpApp.Models
{
    public class GridEntry: IEntryModel
    {
        private int id;
        private string journal;
        private string pages;
        private string title;

        public event EventHandler OnChange;

        public int Id
        {
            get => id;
            set
            {
                this.id = value;
            }
        }
        public string Journal
        {
            get => journal;
            set
            {
                if (journal == value)
                {
                    return;
                }
                this.journal = value;
                OnChange?.Invoke(this, new EventArgs());
            }
        }
        public string Pages
        {
            get => pages;
            set
            {
                this.pages = value;
                OnChange?.Invoke(this, new EventArgs());
            }
        }
        public string Title
        {
            get => title;
            set
            {
                this.title = value;
                OnChange?.Invoke(this, new EventArgs());
            }
        }

        public GridEntry(){}

        public GridEntry(EntryModel entry)
        {
            this.id = entry.Id;
            this.journal = entry.Journal;
            this.pages = entry.Pages;
            this.title = entry.Title;
        }

        public EntryModel ToEntryModel()
        {
            return new EntryModel { Id = id, Journal = journal, Pages = pages, Title = title };
        }

        public override string ToString()
        {
            return $"{id} {journal} {title} {pages}";
        }
    }
}
