
using Wesley.Client.Models.Products;
using Wesley.Client.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class ProductService : IProductService
    {

        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms";

        public ProductService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <returns></returns>
        public async Task<PageData<ProductModel>> GetProductsAsync(int[] categoryIds = null, string key = "", int? terminalid = 0, int wareHouseId = 0, int pageIndex = 0, int pageSize = 20, bool? usablequantity = true, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IProductApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetProductsAsync", storeId, key, categoryIds, terminalid, wareHouseId, pageIndex, pageSize, usablequantity);
                var results = await _makeRequest.StartUseCache(api.GetProductsAsync(storeId, key, categoryIds, terminalid, wareHouseId, pageIndex, pageSize, usablequantity, calToken), cacheKey, force, calToken);
                return results?.Data;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }



        /// <summary>
        /// 获取商品类别
        /// </summary>
        /// <returns></returns>
        public async Task<IList<CategoryModel>> GetAllCategoriesAsync(bool force = false, CancellationToken calToken = default)
        {

            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IProductApi>(URL);
                var cacheKey = RefitServiceBuilder.Cacher("GetAllCategoriesAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetAllCategoriesAsync(storeId, calToken), cacheKey, force, calToken);
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
        /// 获取品牌
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IList<BrandModel>> GetBrandsAsync(string name = "", int pagenumber = 0, int pageSize = 50, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IProductApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetBrandsAsync", storeId, name, pagenumber, pageSize);
                var results = await _makeRequest.StartUseCache(api.GetBrandsAsync(storeId, name, pagenumber, pageSize, calToken), cacheKey, force, calToken);
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
        /// 创建经销商商品档案
        /// </summary>
        /// <param name="data"></param>
        /// <param name="unitPrices"></param>
        /// <returns></returns>
        public async Task<APIResult<ProductModel>> CreateOrUpdateProductAsync(ProductModel data, int id, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IProductApi>(URL);

                var results = await _makeRequest.Start(api.CreateOrUpdateAsync(data, storeId, userId, calToken), calToken);
                return results;
            }
            catch (Exception e)
            {

                e.HandleException();
                return null;
            }
        }



        /// <summary>
        /// 获取经销商商品单位规格属性
        /// </summary>
        /// <returns></returns>
        public async Task<SpecificationModel> GetSpecificationAttributeOptionsAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IProductApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetSpecificationAttributeOptionsAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetSpecificationAttributeOptionsAsync(storeId, calToken), cacheKey, force, calToken);
                return results?.Data;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }

        /// <summary>
        /// 获取调拨商品
        /// </summary>
        /// <param name="type"></param>
        /// <param name="categoryIds"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public async Task<IList<ProductModel>> GetAllocationProductsAsync(int? type, int[] categoryIds, int wareHouseId = 0, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IProductApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetAllocationProductsAsync", storeId, type, categoryIds, wareHouseId);
                var results = await _makeRequest.StartUseCache(api.GetAllocationProductsAsync(storeId, type, categoryIds, wareHouseId, calToken), cacheKey, force, calToken);
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

    }
}
