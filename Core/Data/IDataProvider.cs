using System.Threading.Tasks;
using Core.Models;

namespace Core.Data
{
    public interface IDataProvider
    {
        Task<(EntryModel, string)> CreateEntry(EntryModel entryModel);
        Task<(UserModel, string)> CreateUser(LoginModel login);
        Task<(bool, string)> DeleteEntry(int id);
        Task<(bool, string)> DeleteUser();
        Task<(string content, bool isSuccess)> ExecuteRequest(MbaApiClient.RequestType type, string uri, string json);
        Task<(EntryModel[] entry, string error)> GetEntries();
        Task<(EntryModel entry, string error)> GetEntry(int id);
        Task<(UserModel, string)> GetUser(bool includeEntries = false);
        Task<bool> Login(LoginModel login);
        Task<(EntryModel, string)> UpdateEntry(int id, EntryModel entryModel);
        Task<(UserModel, string)> UpdateUser(UserModel user);
    }
}