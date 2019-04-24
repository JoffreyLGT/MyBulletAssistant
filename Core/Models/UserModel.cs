using System.Collections.Generic;

namespace Core.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public List<EntryModel> Entries{ get; set; }
    }
}
