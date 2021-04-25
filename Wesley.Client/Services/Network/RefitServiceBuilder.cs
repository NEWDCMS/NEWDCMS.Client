using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;
using System;
using System.Net.Http;

namespace Wesley.Client.Services
{
    public class RefitServiceBuilder
    {
        protected static readonly RefitSettings _refitSettings = new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = { new StringEnumConverter() }
            })
        };

        protected static HttpClient _httpClient;


        /// <summary>
        /// 负责定位发送请求的接口的介质
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T Build<T>(string url) where T : class
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
            return RestService.For<T>(_httpClient, _refitSettings);
        }

        public static string Cacher(string name, params object[] args)
        {
            var cacheKey = $"{name}.{string.Join(".", args)}";
            return cacheKey;
        }
    }
}