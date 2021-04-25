using Wesley.Client.Models.Campaigns;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface ICampaignService
    {
        Task<IList<CampaignBuyGiveProductModel>> GetAllCampaigns(string name, int terminalId = 0, int channelId = 0, int wareHouseId = 0, int pagenumber = 0, int pageSize = 50, bool force = false, CancellationToken calToken = default);
    }
}