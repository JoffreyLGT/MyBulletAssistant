using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Entities;

namespace WebApi.Data
{
    public class MbaRepository : IMbaRepository
    {
        private readonly MbaContext context;
        private readonly ILogger<MbaRepository> logger;

        public MbaRepository(MbaContext context, ILogger<MbaRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public void Add<T>(T entity) where T : class
        {
            logger.LogInformation($"Adding an object of type {entity.GetType()} to the context.");
            context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            logger.LogInformation($"Removing an object of type {entity.GetType()} to the context.");
            context.Remove(entity);
        }
        public async Task<bool> SaveChangesAsync()
        {
            logger.LogInformation($"Attempting to save the changes in the context");
            // Only return success if at least one row was changed
            return (await context.SaveChangesAsync()) > 0;
        }

        public async Task<User[]> GetAllUsers(bool includeEntries = false)
        {
            logger.LogInformation($"Getting all Users");
            var query = from user in context.Users
                        select user;
            if (includeEntries)
            {
                query = query.Include(user => user.Entries);
            }
            return await query.ToArrayAsync();
        }
        public async Task<User> GetUserById(string id, bool includeEntries = false)
        {
            logger.LogInformation("Getting user by id.");
            var query = from user in context.Users
                        where user.Id.Equals(id)
                        select user;
            if (includeEntries)
            {
                query = query.Include(user => user.Entries);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmail(string email, bool includeEntries = false)
        {
            logger.LogInformation("Getting user by email.");
            var query = from user in context.Users
                        where user.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)
                        select user;
            if (includeEntries)
            {
                query = query.Include(user => user.Entries);
            }
            return await query.FirstOrDefaultAsync();
        }
        public async Task<Entry[]> GetAllEntries(string userId)
        {
            logger.LogInformation("Getting all Entries for a user.");
            var query = from user in context.Users
                        where user.Id.Equals(userId)
                        select user.Entries;
            var userEntries = await query.FirstOrDefaultAsync();
            return userEntries.ToArray();
        }
        public async Task<Entry> GetEntry(string userId, int entryId)
        {
            logger.LogInformation("The entry by userId and entryId.");
            var query = from user in context.Users
                        where user.Id.Equals(userId)
                        select user.Entries;
            var entries = await query.FirstOrDefaultAsync();

            var userEntry = from entry in entries
                            where entry.Id.Equals(entryId)
                            select entry;

            return userEntry.FirstOrDefault();
        }
    }
}
