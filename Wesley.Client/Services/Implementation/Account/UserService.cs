using Wesley.Client.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class UserService : IUserService
    {
        private readonly MakeRequest _makeRequest;
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/dcms";

        public UserService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
        }

        /// <summary>
        /// 获取经销商业务员
        /// </summary>
        /// <returns></returns>
        public async Task<IList<BusinessUserModel>> GetBusinessUsersAsync(int[] ids, string roleName, bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IUserApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetBusinessUsersAsync", storeId, ids, roleName);
                var results = await _makeRequest.StartUseCache(api.GetBusinessUsersAsync(storeId, ids, roleName, calToken), cacheKey, force, calToken);
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


        public async Task<IList<BusinessUserModel>> GetSubUsersAsync(bool force = false, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IUserApi>(URL);

                var cacheKey = RefitServiceBuilder.Cacher("GetSubUsersAsync", storeId, userId);
                var results = await _makeRequest.StartUseCache(api.GetSubUsersAsync(storeId, userId, calToken), cacheKey, force, calToken);
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
        /// 获取权限记录配置
        /// </summary>
        /// <returns></returns>
        public async Task<IList<PermissionRecordQuery>> GetPermissionRecordSettingAsync(Action<IList<PermissionRecordQuery>> action, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IUserApi>(URL);
                var results = await _makeRequest.Start(api.GetPermissionRecordSettingAsync(storeId, userId, calToken), calToken);
                if (results != null && results?.Code >= 0)
                {
                    var data = results?.Data.ToList();
                    action?.Invoke(data);
                    return data;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public async Task<APIResult<string>> UpLoadFaceImageAsync(string face, CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;
                var api = RefitServiceBuilder.Build<IUserApi>(URL);
                var result = await _makeRequest.Start(api.UpLoadFaceImageAsync(storeId, userId, face, calToken), calToken);
                return result;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }
    }
}
