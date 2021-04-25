using Wesley.Client.Models.Products;
using Wesley.Client.Paging;
using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms", true, isAutoRegistrable: false), Cache(CacheMode.GetAndFetch, "00:05:00"), Trace]
    //[Headers("Authorization: Bearer")]
    public interface IProductApi
    {
        [Post("/archives/product/createOrUpdateProduct/{storeId}/{userId}")]
        Task<APIResult<ProductModel>> CreateOrUpdateAsync(ProductModel data, int storeId, int userId, CancellationToken calToken = default);

        [Get("/archives/category/getAllCategories/{storeId}")]
        Task<APIResult<IList<CategoryModel>>> GetAllCategoriesAsync(int storeId, CancellationToken calToken = default);

        [Get("/archives/product/getAllocationProducts/{storeId}")]
        Task<APIResult<IList<ProductModel>>> GetAllocationProductsAsync(int storeId, int? type, [Query(CollectionFormat.Multi)] int[] categoryIds = null, int wareHouseId = 0, CancellationToken calToken = default);

        [Get("/archives/getAllBrands/{storeId}")]
        Task<APIResult<IList<BrandModel>>> GetBrandsAsync(int storeId, string name = "", int pagenumber = 0, int pageSize = 50, CancellationToken calToken = default);

        [Get("/archives/product/getProducts/{storeId}")]
        Task<APIResult<PageData<ProductModel>>> GetProductsAsync(int storeId, string key = "", [Query(CollectionFormat.Multi)] int[] categoryIds = null, int? terminalid = 0, int wareHouseId = 0, int pageIndex = 0, int pageSize = 20, bool? usablequantity = true, CancellationToken calToken = default);

        [Get("/archives/product/getSpecificationAttributeOptions/{storeId}")]
        Task<APIResult<SpecificationModel>> GetSpecificationAttributeOptionsAsync(int storeId, CancellationToken calToken = default);
    }
}