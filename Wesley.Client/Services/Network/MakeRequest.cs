using Akavache;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    /// <summary>
    /// 负责网络请求服务
    /// </summary>
    public class MakeRequest
    {
        private readonly IConnectivityHandler _connectivityHandler;
        private readonly ICacheHandler _cacheHandler;

        public MakeRequest(IConnectivityHandler connectivityHandler, ICacheHandler cacheHandler)
        {
            _connectivityHandler = connectivityHandler;
            _cacheHandler = cacheHandler;
        }

        protected readonly AsyncRetryPolicy _policy = Policy
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: (attempt) => TimeSpan.FromSeconds(5));

        /// <summary>
        /// 直接执行请求的方法（不使用缓存）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<T> Start<T>(Task<T> task, CancellationToken token = default)
        {
            try
            {
                if (!_connectivityHandler.IsConnected())
                {
                    return Task.FromResult(default(T));
                }

                return _policy.ExecuteAsync(async (token) =>
                {
                    return await task;
                }, token);
            }
            catch (HttpRequestException)
            {
                return Task.FromResult(default(T));
            }
            catch (Exception)
            {
                return Task.FromResult(default(T));
            }
        }

        /// <summary>
        /// 缓存执行请求的方法（按需缓存）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="force">是否强制缓存</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<T> StartUseCache<T>(Task<T> task, string cacheKey, bool force = false, CancellationToken token = default)
        {
            try
            {
                if (!_connectivityHandler.IsConnected())
                {
                    return await Task.FromResult(default(T));
                }

                /*
                BlobCache.LocalMachine  –常规缓存的数据。从服务器检索的对象，即新闻项，用户列表等。
                BlobCache.UserAccount  –用户在您的应用程序中执行的设置。一些平台将此同步到云。
                BlobCache.Secure  –如果要保存敏感数据，则在此放置数据。
                BlobCache.InMemory  –顾名思义；每当您的应用被杀死时，它就会将数据保存在内存中，其中的数据也将保留在内存中。
                 */

                var cache = BlobCache.LocalMachine;

                if (force)
                {
                    //await _cacheHandler.Remove(cacheKey);
                    await cache.Invalidate(cacheKey);
                }

                //var postsCache = await cache.GetOrFetchObject<T>(cacheKey, async () =>
                // {
                //     var newPosts = await _policy.ExecuteAsync(async () =>
                //     {
                //         return await task;
                //     }).ConfigureAwait(false);
                //     await cache.InsertObject<T>(cacheKey, newPosts);
                //     return newPosts;
                // });

                IObservable<T> cachedConferences = cache.GetAndFetchLatest(cacheKey, () =>
                {
                    return _policy.ExecuteAsync(async (token) =>
                    {
                        return await task;
                    }, token);
                },
                offset =>
                {
                    TimeSpan elapsed = DateTimeOffset.Now - offset;
                    //var invalidateCache = (force || elapsed > new TimeSpan(hours: 0, minutes: 5, seconds: 0));
                    return elapsed > new TimeSpan(hours: 0, minutes: 5, seconds: 0);
                });

                return await cachedConferences.FirstOrDefaultAsync();
            }
            catch (HttpRequestException)
            {
                return await Task.FromResult(default(T));
            }
            catch (Exception)
            {
                return await Task.FromResult(default(T));
            }
        }
    }
}