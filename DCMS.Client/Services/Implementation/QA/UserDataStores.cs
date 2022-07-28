using DCMS.Client.Models.QA;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCMS.Client.Services
{
    public class UserDataStores : IUserDataStores
    {
        private readonly List<User> _users;

        public UserDataStores()
        {
            _users = new List<User>
            {
                new User("13002929017", "管理员", "小陈", "业务咨询","face.jpg", 5, 230){IsOnline = true,ConversationId = "ece4f4a60b764339b94a07c84e338a27" },
                new User("13002929027", "客服1", "小周", "技术支持.", "face.jpg", 10, 1500){ConversationId = "eae4f4a60b764339b94a07c84e338a27"  },
                new User("13002929037", "客服2", "小王", "问题反馈","face.jpg", 15, 100){ConversationId = "ebe4f4a60b764339b94a07c84e338a27"  },
                new User("13002929047", "客服3", "小张", "配送和物流查询","face.jpg", 2, 10){ConversationId = "ede4f4a60b764339b94a07c84e338a27"  },
                new User("13002929057", "客服4", "小李", "汇款和发票咨询", "face.jpg", 5, 230){ConversationId = "efe4f4a60b764339b94a07c84e338a27"  }
            };
        }

        public Task<bool> AddItemAsync(User item)
        {
            _users.Add(item);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            var user = _users.Where(u => u.Id == id);
            if (!user.Any())
                return Task.FromResult(false);
            _users.Remove(user.FirstOrDefault());
            return Task.FromResult(true);
        }

        public Task<User> GetItemAsync(string id)
        {
            return Task.FromResult(_users.Where(u => u.Id == id).FirstOrDefault());
        }

        public Task<IEnumerable<User>> GetItemsAsync(bool forceRefresh = false)
        {
            return Task.FromResult((IEnumerable<User>)_users);
        }

        public Task<bool> UpdateItemAsync(User item)
        {
            var user = _users.Where(u => u.Id == item.Id);
            if (!user.Any())
                return Task.FromResult(false);

            _users.Remove(_users.Where(u => u.Id == item.Id).FirstOrDefault());
            _users.Add(item);
            return Task.FromResult(true);
        }
    }
}
