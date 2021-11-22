using Wesley.Client.Models.Users;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        //private readonly IOperatingSystemVersionProvider _operatingSystemVersionProvider;
        private readonly MakeRequest _makeRequest;
        //LogOutAsync
        private static string URL => GlobalSettings.BaseEndpoint + "api/v3/Wesley/auth";

        public AuthenticationService(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
            //_operatingSystemVersionProvider = operatingSystemVersionProvider;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<int> LoginAsync(string userName, string password, CancellationToken calToken = default)
        {
            var model = new LoginModel
            {
                UserName = userName,
                Password = password,
                //AppId = _operatingSystemVersionProvider.GetDeviceId()
            };

            var result = new APIResult<UserAuthenticationModel>();

            try
            {
                var api = RefitServiceBuilder.Build<IAuthenticationApi>(URL, false);
                result = await _makeRequest.Start(api.LoginAsync(model, calToken), calToken);
                if (result != null)
                {
                    if (result != null && result.Code == 1)
                    {
                        var data = result?.Data;
                        Settings.Password = password;
                        Settings.UserName = userName;
                        Settings.AccessToken = data?.AccessToken;
                        Settings.UUID = data?.AppId;
                        Settings.UserId = data?.Id ?? 0;
                        Settings.StoreId = data?.StoreId ?? 0;
                        Settings.UserMobile = data?.MobileNumber;
                        Settings.StoreName = data?.StoreName;
                        Settings.UserRealName = data?.UserRealName;
                        Settings.FaceImage = data?.FaceImage;
                        Settings.UserEmall = data?.Email;

                        Settings.DealerNumber = data?.DealerNumber;
                        Settings.MarketingCenter = data?.MarketingCenter;
                        Settings.MarketingCenterCode = data?.MarketingCenterCode;
                        Settings.SalesArea = data?.SalesArea;
                        Settings.SalesAreaCode = data?.SalesAreaCode;
                        Settings.BusinessDepartment = data?.BusinessDepartment;
                        Settings.BusinessDepartmentCode = data?.BusinessDepartmentCode;

                        //Roles
                        if (result.Data.Roles != null && result.Data.Roles.Count > 0)
                        {
                            try
                            {
                                var roles = result.Data.Roles;
                                //用户角色
                                Settings.DefaultRole = roles.FirstOrDefault()?.Name;
                                Settings.AvailableUserRoles = JsonConvert.SerializeObject(roles);
                                //用户权限
                                Settings.AvailablePermissionRecords = JsonConvert.SerializeObject(result.Data.PermissionRecords);
                            }
                            catch (Exception ex)
                            {
                                result.Message = $"{ex.Message}";
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.HandleException();
            }

            return result.Code;
        }

        /// <summary>
        /// 自动登录
        /// </summary>
        /// <param name="cts"></param>
        /// <returns></returns>
        public async Task<bool> AutoLoginAsync(CancellationToken calToken = default)
        {
            bool isAuthenticated = false;
            if (string.IsNullOrEmpty(Settings.UserMobile) || string.IsNullOrEmpty(Settings.Password))
            {
                return false;
            }

            try
            {
                Settings.IsInitData = false;
                var rcode = await LoginAsync(Settings.UserMobile, Settings.Password);
                if (rcode == 1)
                {
                    isAuthenticated = true;
                }
            }
            catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

            //认证成功
            if (isAuthenticated)
            {
                var _globalService = App.Resolve<IGlobalService>();
                _globalService?.GetAPPFeatures();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 注销//api/v3/Wesley/auth/user/logout/{userId}
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LogOutAsync(CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var token = new RevokeTokenRequest() { Token = Settings.AccessToken };
                var api = RefitServiceBuilder.Build<IAuthenticationApi>(URL);
                var result = await _makeRequest.Start(api.LogOutAsync(token, storeId, userId, calToken), calToken);
                if (result != null)
                {
                    if (result != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                try
                {
                    Settings.ClearEverything();
                }
                catch (System.Exception) { }
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<UserModel> CheckStatusAsync(CancellationToken calToken = default)
        {
            try
            {
                int storeId = Settings.StoreId;
                int userId = Settings.UserId;

                var api = RefitServiceBuilder.Build<IAuthenticationApi>(URL);
                var result = await _makeRequest.Start(api.CheckStatusAsync(storeId, userId, calToken), calToken);
                if (result.Data != null && result.Data.Id > 0)
                    return result.Data;
                else
                    return null;
            }
            catch (Exception e)
            {
                e.HandleException();
                return null;
            }
        }

        public async Task<UserAuthenticationModel> RefreshTokenAsync(CancellationToken calToken = default)
        {
            try
            {
                var api = RefitServiceBuilder.Build<IAuthenticationApi>(URL, false);
                var result = await _makeRequest.Start(api.RefreshTokenAsync(Settings.AccessToken, calToken), calToken);
                if (result.Data != null && result.Data.Id > 0)
                    return result.Data;
                else
                    return null;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return null;
            }
            catch (Refit.ApiException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> QRLoginAsync(string uuid, CancellationToken calToken = default)
        {
            try
            {
                var api = RefitServiceBuilder.Build<IAuthenticationApi>(URL);
                var result = await _makeRequest.Start(api.QRLoginAsync(uuid, Settings.UserId, calToken), calToken);
                if (result != null)
                    return result.Data;
                else
                    return true;
            }
            catch (Exception e)
            {
                e.HandleException();
                return false;
            }
        }

    }
}

