using DCMS.Client.Models.Finances;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DCMS.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/finances/costContract", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface ICostContractApi
    {
        [Get("/getbills/{storeId}/{customerId}/{employeeId}")]
        Task<APIResult<IList<CostContractBillModel>>> GetCostContractsAsync(int storeId, int? makeuserId, int? customerId, string customerName, int? employeeId, string billNumber, string remark, DateTime? startTime = null, DateTime? endTime = null, bool? auditedStatus = null, bool? showReverse = null, int pagenumber = 0, int pageSize = 50, CancellationToken calToken = default);

        [Get("/auditing/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> AuditingAsync(int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Post("/createorupdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<CostContractUpdateModel>> CreateOrUpdateAsync(CostContractUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Get("/getCostContractsByAccountingOptionId/{storeId}/{customerId}/{accountCodeTypeId}")]
        Task<APIResult<IList<CostContractBindingModel>>> GetCostContractsAsync(int storeId, int? customerId, int? accountOptionId, int? accountCodeTypeId, int year, int month, int? contractType = 0, bool? auditedStatus = null, bool? showReverse = null, int pageIndex = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/getCostContractBill/{storeId}/{billId}/{userId}")]
        Task<APIResult<CostContractBillModel>> GetBillAsync(int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/reverse/{storeId}/{userId}/{billId}")]
        Task<APIResult<dynamic>> ReverseAsync(int storeId, int userId, int billId = 0, string remark = "", CancellationToken calToken = default);
    }
}