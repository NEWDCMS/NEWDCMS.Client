using DCMS.Client.Enums;
using DCMS.Client.Models;
using DCMS.Client.Models.Census;
using DCMS.Client.Models.Sales;
using Newtonsoft.Json;
//using Shiny;
//using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DCMS.Client
{
    /*
    /// <summary>
    /// 用于客户端本地数据存储
    /// </summary>
    public class LocalDatabase : SQLiteAsyncConnection
    {
        public LocalDatabase(IPlatform platform) : base(Path.Combine(platform.AppData.FullName, "dcms.db"))
        {
            try
            {
                var conn = this.GetConnection();
                conn.CreateTable<JobLog>();
                conn.CreateTable<VisitStore>();
                conn.CreateTable<TrackingModel>();
                conn.CreateTable<NotificationEvent>();
                conn.CreateTable<PushEvent>();
                conn.CreateTable<ErrorLog>();
                conn.CreateTable<MessageInfo>();
                conn.CreateTable<CacheBillData>();
                conn.CreateTable<CachePaymentMethod>();

            }
            catch (Exception) { }
        }

        public string DatabasePathPath => DatabasePath;

        public AsyncTableQuery<JobLog> JobLogs => this.Table<JobLog>();
        public AsyncTableQuery<VisitStore> VisitStoreEvents => this.Table<VisitStore>();
        public AsyncTableQuery<NotificationEvent> NotificationEvents => this.Table<NotificationEvent>();
        public AsyncTableQuery<PushEvent> PushEvents => this.Table<PushEvent>();
        public AsyncTableQuery<ErrorLog> ErrorLogEvents => this.Table<ErrorLog>();
        public AsyncTableQuery<MessageInfo> MessageInfoEvents => this.Table<MessageInfo>();
        public AsyncTableQuery<TrackingModel> LocationSyncEvents => this.Table<TrackingModel>();


        /// <summary>
        /// 对象保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task SaveItemAsync<T>(T item)
        {
            await this.InsertAsync(item);
        }

        public async Task ResetErrorLogs()
        {
            await this.DropTableAsync<ErrorLog>();
            await this.CreateTableAsync<ErrorLog>();
        }

        public async Task ResetMessageInfos()
        {
            await this.DropTableAsync<MessageInfo>();
            await this.CreateTableAsync<MessageInfo>();
        }

        public async Task ResetLocationSyncEvents()
        {
            await this.DropTableAsync<TrackingModel>();
            await this.CreateTableAsync<TrackingModel>();
        }
        public async Task ResetCacheDataStores()
        {
            await this.DropTableAsync<CacheBillData>();
            await this.CreateTableAsync<CacheBillData>();
            await this.DropTableAsync<CachePaymentMethod>();
            await this.CreateTableAsync<CachePaymentMethod>();
        }

        public async Task ResetVisitStores()
        {
            await this.DropTableAsync<VisitStore>();
            await this.CreateTableAsync<VisitStore>();
        }

        #region 缓存数据
        /// <summary>
        /// 获取缓存单据数据
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<AbstractBill, PaymentMethodBaseModel>> GetAbstractBillByType(BillTypeEnum typeId)
        {
            try
            {

                var result = await this.Table<CacheBillData>()
                    .Where(c => c.TypeId == (int)typeId)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefaultAsync();
                if (null == result)
                {
                    return null;
                }
                AbstractBill cacheData = null;
                switch (typeId)
                {
                    case BillTypeEnum.None:
                        break;
                    case BillTypeEnum.ExchangeBill:
                        break;
                    case BillTypeEnum.SaleReservationBill:
                        cacheData = JsonConvert.DeserializeObject<SaleReservationBillModel>(result?.DataValue);
                        break;
                    case BillTypeEnum.SaleBill:
                        cacheData = JsonConvert.DeserializeObject<SaleBillModel>(result?.DataValue);
                        break;
                    case BillTypeEnum.ReturnReservationBill:
                        cacheData = JsonConvert.DeserializeObject<ReturnReservationBillModel>(result?.DataValue);
                        break;
                    case BillTypeEnum.ReturnBill:
                        cacheData = JsonConvert.DeserializeObject<ReturnBillModel>(result?.DataValue);
                        break;
                    case BillTypeEnum.CarGoodBill:
                        break;
                    case BillTypeEnum.FinanceReceiveAccount:
                        break;
                    case BillTypeEnum.PickingBill:
                        break;
                    case BillTypeEnum.DispatchBill:
                        break;
                    case BillTypeEnum.ChangeReservation:
                        break;
                    case BillTypeEnum.PurchaseReservationBill:
                        break;
                    case BillTypeEnum.PurchaseBill:
                        break;
                    case BillTypeEnum.PurchaseReturnReservationBill:
                        break;
                    case BillTypeEnum.PurchaseReturnBill:
                        break;
                    case BillTypeEnum.BackStockBill:
                        break;
                    case BillTypeEnum.AllocationBill:
                        break;
                    case BillTypeEnum.InventoryProfitLossBill:
                        break;
                    case BillTypeEnum.CostAdjustmentBill:
                        break;
                    case BillTypeEnum.ScrapProductBill:
                        break;
                    case BillTypeEnum.InventoryAllTaskBill:
                        break;
                    case BillTypeEnum.InventoryPartTaskBill:
                        break;
                    case BillTypeEnum.CombinationProductBill:
                        break;
                    case BillTypeEnum.SplitProductBill:
                        break;
                    case BillTypeEnum.InventoryReportBill:
                        break;
                    case BillTypeEnum.CashReceiptBill:
                        break;
                    case BillTypeEnum.PaymentReceiptBill:
                        break;
                    case BillTypeEnum.AdvanceReceiptBill:
                        break;
                    case BillTypeEnum.AdvancePaymentBill:
                        break;
                    case BillTypeEnum.CostExpenditureBill:
                        break;
                    case BillTypeEnum.CostContractBill:
                        break;
                    case BillTypeEnum.FinancialIncomeBill:
                        break;
                    case BillTypeEnum.AllLoadBill:
                        break;
                    case BillTypeEnum.ZeroLoadBill:
                        break;
                    case BillTypeEnum.AllZeroMergerBill:
                        break;
                    case BillTypeEnum.AccountingVoucher:
                        break;
                    case BillTypeEnum.StockReport:
                        break;
                    case BillTypeEnum.SaleSummeryReport:
                        break;
                    case BillTypeEnum.TransferSummaryReport:
                        break;
                    case BillTypeEnum.SaleSummeryProductReport:
                        break;
                    case BillTypeEnum.RecordingVoucher:
                        break;
                    case BillTypeEnum.LoanGoodsBill:
                        break;
                    case BillTypeEnum.ReturnGoodsBill:
                        break;
                    case BillTypeEnum.Other:
                        break;
                    default:
                        break;
                }
                PaymentMethodBaseModel payment = null;
                if (cacheData != null)
                {
                    var billGuid = cacheData?.GUID;
                    var paymentCache = await this.Table<CachePaymentMethod>()
                    .Where(c => c.BillTypeId == (int)typeId && billGuid == c.BillGuid)
                    .FirstOrDefaultAsync();
                    if (null != paymentCache && !string.IsNullOrEmpty(paymentCache?.DataValue))
                    {
                        payment = JsonConvert.DeserializeObject<PaymentMethodBaseModel>(paymentCache?.DataValue);
                    }
                }
                return Tuple.Create(cacheData, payment);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 缓存销售单数据
        /// </summary>
        /// <returns></returns>
        public async Task SetCacheDataInfo(BillTypeEnum typeId, AbstractBill bill, PaymentMethodBaseModel payment)
        {
            try
            {
                var result = await this.Table<CacheBillData>()
                    .Where(c => c.TypeId == (int)typeId)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefaultAsync();
                if (result != null)
                    await this.DeleteAsync(result);

                var cacheData = new CacheBillData
                {
                    TypeId = (int)typeId,
                    DataValue = JsonConvert.SerializeObject(bill)
                };
                await this.InsertAsync(cacheData);

                var paymentOldCache = await this.Table<CachePaymentMethod>()
                    .Where(c => c.BillTypeId == (int)typeId)
                    .ToListAsync();

                if (paymentOldCache != null && paymentOldCache.Any())
                    foreach (var item in paymentOldCache)
                    {
                        await this.DeleteAsync(item);
                    }
                if (payment != null)
                {
                    var cachePaymentData = new CachePaymentMethod
                    {
                        BillTypeId = (int)typeId,
                        BillGuid = bill.GUID,
                        DataValue = JsonConvert.SerializeObject(payment)
                    };
                    await this.InsertAsync(cachePaymentData);
                }
            }
            catch (Exception)
            {
            }
        }
        public async Task ClearCacheDataByType(BillTypeEnum typeId)
        {
            try
            {
                var result = await this.Table<CacheBillData>()
                    .Where(c => c.TypeId == (int)typeId)
                    .ToListAsync(); ;
                if (result != null && result.Any())
                    foreach (var item in result)
                    {
                        await this.DeleteAsync(item);
                    }

                var paymentOldCache = await this.Table<CachePaymentMethod>()
                    .Where(c => c.BillTypeId == (int)typeId)
                    .ToListAsync();

                if (paymentOldCache != null && paymentOldCache.Any())
                    foreach (var item in paymentOldCache)
                    {
                        await this.DeleteAsync(item);
                    }
            }
            catch (Exception)
            {
            }
        }
        #endregion


        #region 签到

        public async Task<bool> CheckVisitStore(int terminalId)
        {
            try
            {
                var results = await this.Table<VisitStore>()
                    .Where(c => c.TerminalId == terminalId)
                    .ToListAsync();

                bool exits = false;
                if (results.Any())
                {
                    foreach (var s in results)
                    {
                        if (s.SignOutDateTime.ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                        {
                            exits = true;
                            break;
                        }
                    }
                }
                return exits;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region 位置

        public async Task RemoveTopLocation()
        {
            try
            {
                var result = await this.Table<TrackingModel>().FirstOrDefaultAsync();
                if (result != null)
                    await this.DeleteAsync(result);
            }
            catch (Exception)
            {

            }
        }

        public async Task<List<TrackingModel>> GetAllLocations()
        {
            return await this.Table<TrackingModel>().ToListAsync();
        }

        #endregion

        #region 消息

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public async Task RemoveMessageInfo(int msgId)
        {
            var result = await this.Table<MessageInfo>().Where(c => c.Id == msgId).FirstOrDefaultAsync();
            if (result != null)
            {
                await this.DeleteAsync(result);
            }
        }

        /// <summary>
        /// 设置已读
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        public async Task SetPending(int msgId, bool read)
        {
            var result = await this.Table<MessageInfo>().Where(c => c.Id == msgId).FirstOrDefaultAsync();
            if (result != null)
            {
                result.IsRead = read;
                await this.UpdateAsync(result);
            }
        }

        /// <summary>
        /// 获取全部消息
        /// </summary>
        /// <returns></returns>
        public async Task<List<MessageInfo>> GetAllMessageInfos()
        {
            return await this.Table<MessageInfo>().ToListAsync();
        }

        /// <summary>
        /// 获取全部消息
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public async Task<List<MessageInfo>> GetAllMessageInfos(int[] types)
        {
            try
            {
                List<MessageInfo> messageInfos = await this
                          .Table<MessageInfo>()
                          .Where(c => types.Contains(c.MTypeId))
                          .OrderByDescending(c => c.Id)
                          .ToListAsync();

                return messageInfos;
            }
            catch (Exception)
            {
                return new List<MessageInfo>();
            }
        }

        #endregion
    }

    */
}
