using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Entry
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        [Required]
        public string Journal { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Pages { get; set; }
    }
}
