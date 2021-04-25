using Wesley.Client.Models.QA;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Wesley.Client.Services
{
    public interface IUserDataStores
    {
        Task<bool> AddItemAsync(User item);
        Task<bool> DeleteItemAsync(string id);
        Task<User> GetItemAsync(string id);
        Task<IEnumerable<User>> GetItemsAsync(bool forceRefresh = false);
        Task<bool> UpdateItemAsync(User item);
    }
}