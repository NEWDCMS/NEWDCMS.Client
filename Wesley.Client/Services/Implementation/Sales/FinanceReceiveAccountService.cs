
using Wesley.Client.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Wesley.Client.Services.Sales
{
    public class FinanceReceiveAccountService : IFinanceReceiveAccountService
    {
        private readonly MakeRequest _makeRequest;
        //
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms/sales/financereceiveaccount";

        public FinanceReceiveAccountService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取待上交对账单汇总
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="businessUserId"></param>
        /// <param name="payeer"></param>
        /// <param name="accountingOptionId"></param>
        /// <param name="billNumber"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IList<FinanceReceiveAccountBillModel>> GetFinanceReceiveAccounts(DateTime? start, DateTime? end, int? businessUserId, int? payeer, int? accountingOptionId, string billNumber = "", int pageIndex = 0, int pageSize = 20, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IFinanceReceiveAccountApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetFinanceReceiveAccounts", storeId,
                    start,
                    end,
                    businessUserId,
                    payeer,
                    accountingOptionId,
                    billNumber,
                    pageIndex,
                    pageSize);

                var results = await _makeRequest.StartUseCache(api.GetFinanceReceiveAccounts(storeId,
                    start,
                    end,
                    businessUserId,
                    payeer,
                    accountingOptionId,
                    billNumber,
                    pageIndex,
                    pageSize, calToken),
                    cacheKey, force, calToken);

                if (results != null && results?.Code >= 0)
                    return results?.Data.ToList();
                else
                    return null;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }


        /// <summary>
        /// 上交对账单(批量)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="store"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<APIResult<FinanceReceiveAccountBillSubmitModel>> SubmitAccountStatementAsync(FinanceReceiveAccountBillSubmitModel data, int id, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IFinanceReceiveAccountApi>(URL);
                var results = await _makeRequest.Start(api.SubmitAccountStatementAsync(data, storeId, userId, id, calToken), calToken);
                return results;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }
    }
}
