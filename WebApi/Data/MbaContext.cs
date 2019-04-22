using Microsoft.EntityFrameworkCore;
using WebApi.Data.Entities;

namespace WebApi.Data
{
    public class MbaContext : DbContext
    {
        public MbaContext(DbContextOptions<MbaContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Entry> Entries { get; set; }
    }
}
