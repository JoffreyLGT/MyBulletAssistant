using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class EntriesIndexViewModel
    {
        public string JournalFilter { get; set; } = "";
        public string TitleFilter { get; set; } = "";
        public List<string> Journals { get; set; }
        public List<Entry> Entries { get; set; }
    
    }
}
