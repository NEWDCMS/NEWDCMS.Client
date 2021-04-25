using Wesley.Client.Models.Finances;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/finances/costexpenditurebill", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    //[Headers("Authorization: Bearer")]
    public interface ICostExpenditureApi
    {
        [Get("/auditing/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> AuditingAsync(int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Post("/createorupdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<CostExpenditureUpdateModel>> CreateOrUpdateAsync(CostExpenditureUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Get("/getbills/{storeId}/{customerId}/{employeeId}")]
        Task<APIResult<IList<CostExpenditureBillModel>>> GetCostExpendituresAsync(int storeId, int? makeuserId, int? customerId, string customerName, int? employeeId, string billNumber, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, bool? showReverse = null, bool sortByAuditedTime = false, int? sign = null, int pagenumber = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/getCostExpenditureBill/{storeId}/{billId}/{userId}")]
        Task<APIResult<CostExpenditureBillModel>> GetBillAsync(int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/reverse/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> ReverseAsync(int storeId, int userId, int billId = 0, CancellationToken calToken = default);
    }
}