using Wesley.Client.Models.QA;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IConversationsDataStore
    {
        Task<bool> AddItemAsync(Conversation item);
        Task<bool> DeleteItemAsync(string id);
        Task<IEnumerable<Conversation>> GetConversationsForUser(string userId);
        Task<Conversation> GetItemAsync(string id);
        Task<IEnumerable<Conversation>> GetItemsAsync(bool forceRefresh = false);
        Task<bool> UpdateItemAsync(Conversation item);
    }
}