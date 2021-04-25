using Wesley.Client.Models.QA;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IMessagesDataStore
    {
        Task<bool> AddItemAsync(Message item);
        Task<bool> DeleteItemAsync(string id);
        Task<Message> GetItemAsync(string id);
        Task<IEnumerable<Message>> GetItemsAsync(bool forceRefresh = false);
        Task<IEnumerable<Message>> GetMessagesForConversation(string conversationId);
        Task<bool> UpdateItemAsync(Message item);
    }
}