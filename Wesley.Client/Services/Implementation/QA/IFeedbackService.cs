using Wesley.Client.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services.QA
{
    public interface IFeedbackService
    {
        Task<APIResult<FeedBack>> CreateOrUpdateAsync(FeedBack data, CancellationToken calToken = default);
        Task<APIResult<MarketFeedback>> CreateOrUpdateMarketAsync(MarketFeedback data, CancellationToken calToken = default);
    }
}