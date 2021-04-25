using Wesley.Client.Models.Products;
using Wesley.Client.Paging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IProductService
    {
        Task<APIResult<ProductModel>> CreateOrUpdateProductAsync(ProductModel data, int id, CancellationToken calToken = default);
        Task<IList<CategoryModel>> GetAllCategoriesAsync(bool force = false, CancellationToken calToken = default);
        Task<IList<ProductModel>> GetAllocationProductsAsync(int? type, int[] categoryIds, int wareHouseId = 0, bool force = false, CancellationToken calToken = default);
        Task<IList<BrandModel>> GetBrandsAsync(string name = "", int pagenumber = 0, int pageSize = 50, bool force = false, CancellationToken calToken = default);
        Task<PageData<ProductModel>> GetProductsAsync(int[] categoryIds, string key = "", int? terminalid = 0, int wareHouseId = 0, int pageIndex = 0, int pageSize = 20, bool? usablequantity = true, bool force = false, CancellationToken calToken = default);
        Task<SpecificationModel> GetSpecificationAttributeOptionsAsync(bool force = false, CancellationToken calToken = default);
    }
}