using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class Entry
    {
        public int Id { get; set; }

        [Required]
        public string Journal { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Pages { get; set; }
    }
}
