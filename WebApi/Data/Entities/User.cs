using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Data.Entities
{
    public class User : IdentityUser
    {
        public ICollection<Entry> Entries { get; set; }
    }
}
