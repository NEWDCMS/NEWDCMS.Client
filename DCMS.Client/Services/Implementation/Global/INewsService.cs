using Wesley.Client.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface INewsService
    {
        Task<IList<NewsInfoModel>> GetNewsAsync(bool force = false, CancellationToken calToken = default);
        Task<NewsInfoModel> GetNewsAsync(int newsId, bool force = false, CancellationToken calToken = default);
        Task<IList<NewsInfoModel>> GetTopNewsAsync(bool force = false, CancellationToken calToken = default);
    }
}