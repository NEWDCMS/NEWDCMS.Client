using Wesley.Client.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public interface IUserService
    {
        Task<IList<BusinessUserModel>> GetBusinessUsersAsync(int[] ids, string roleName, bool force = false, CancellationToken calToken = default);
        Task<IList<BusinessUserModel>> GetSubUsersAsync(bool force = false, CancellationToken calToken = default);
        Task<IList<PermissionRecordQuery>> GetPermissionRecordSettingAsync(Action<IList<PermissionRecordQuery>> action, CancellationToken calToken = default);
        Task<APIResult<string>> UpLoadFaceImageAsync(string face, CancellationToken calToken = default);
    }
}