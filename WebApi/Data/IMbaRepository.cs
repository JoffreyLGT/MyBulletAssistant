using System.Threading.Tasks;
using WebApi.Data.Entities;

namespace WebApi.Data
{
    public interface IMbaRepository
    {
        // General
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();

        // Users
        Task<User[]> GetAllUsers(bool includeEntries = false);
        Task<User> GetUserById(string id, bool includeEntries = false);
        Task<User> GetUserByEmail(string email, bool includeEntries = false);

        // Entries
        Task<Entry[]> GetAllEntries(string userId);
        Task<Entry> GetEntry(string userId, int entryId);
    }
}
