using DCMS.Client.Models.Users;
using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DCMS.Client.Services
{

    //[WebApi(GlobalSettings.BaseEndpoint + "api/v3/dcms", true, isAutoRegistrable: false), Cache(false), Trace]
    [Headers("Authorization: Bearer")]
    public interface IUserApi
    {
        [Get("/users/getBusinessUsers/{storeId}")]
        Task<APIResult<IList<BusinessUserModel>>> GetBusinessUsersAsync(int storeId, [Query(CollectionFormat.Multi)] int[] ids, string roleName, CancellationToken calToken = default);

        [Get("/users/getSubUsers/{storeId}")]
        Task<APIResult<IList<BusinessUserModel>>> GetSubUsersAsync(int storeId, int userId, CancellationToken calToken = default);

        //http://api.jsdcms.com/api/v3/dcms/auth/user/permission/5350?store=361
        [Get("/auth/user/permission/{userId}")]
        Task<APIResult<IList<PermissionRecordQuery>>> GetPermissionRecordSettingAsync(int store, int userId, CancellationToken calToken = default);

        //
        [Post("/auth/user/profiles/updateFaceImage/{userId}")]
        //[Headers("Authorization: Bearer")]
        Task<APIResult<string>> UpLoadFaceImageAsync(int store, int userId, string image, CancellationToken calToken = default);
    }
}