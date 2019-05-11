using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class EntryModel : IEntryModel
    {
        public int Id { get; set; }
        public string Journal { get; set; }
        public string Title { get; set; }
        public string Pages { get; set; }
    }
}
