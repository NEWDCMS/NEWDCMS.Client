using Wesley.Client.Models.WareHouses;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms/warehouse/inventoryPartTask", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    [Headers("Authorization: Bearer")]
    public interface IInventoryApi
    {

        [Get("/cancelTakeInventory/{storeId}/{userId}/{billId}")]
        Task<APIResult<bool>> CancelTakeInventoryAsync(int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/CheckInventory/{storeId}/{userId}/{wareHouseId}")]
        Task<APIResult<IList<InventoryPartTaskBillModel>>> CheckInventoryAsync(int storeId, int userId, int wareHouseId, CancellationToken calToken = default);

        [Post("/createOrUpdate/{storeId}/{userId}/{billId}")]
        Task<APIResult<InventoryPartTaskUpdateModel>> CreateOrUpdateAsync(InventoryPartTaskUpdateModel data, int storeId, int userId, int billId = 0, CancellationToken calToken = default);

        [Get("/getbills/{storeId}/{inventoryPerson}/{wareHouseId}")]
        Task<APIResult<IList<InventoryPartTaskBillModel>>> GetInventoryAllsAsync(int storeId, int? makeuserId, int? inventoryPerson, int? wareHouseId, string billNumber, string remark, bool? auditedStatus = null, DateTime? startTime = null, DateTime? endTime = null, int? inventoryStatus = -1, bool? showReverse = null, bool? sortByCompletedTime = null, int pagenumber = 0, int pageSize = 20, CancellationToken calToken = default);

        [Get("/getInventoryPartTaskBill/{storeId}/{userId}/{billId}")]
        Task<APIResult<InventoryPartTaskBillModel>> GetInventoryPartTaskBillAsync(int storeId, int userId, int billId, CancellationToken calToken = default);

        [Get("/setInventoryCompleted/{storeId}/{userId}/{billId}")]
        Task<APIResult<bool>> SetInventoryCompletedAsync(int storeId, int userId, int billId, CancellationToken calToken = default);
    }
}