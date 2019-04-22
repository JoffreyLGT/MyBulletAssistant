using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.Entities
{
    public class User
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public ICollection<Entry> Entries { get; set; }
    }
}
