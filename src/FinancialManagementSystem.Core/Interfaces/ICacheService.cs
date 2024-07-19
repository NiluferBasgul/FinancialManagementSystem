using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialManagementSystem.Core.Interfaces
{

    namespace FinancialManagementSystem.Core.Services
    {
        public interface ICacheService
        {
            Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
            Task RemoveAsync(string key);
        }

        public class CacheService : ICacheService
        {
            private readonly IDistributedCache _cache;

            public CacheService(IDistributedCache cache)
            {
                _cache = cache;
            }

            public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
            {
                var cachedResult = await _cache.GetStringAsync(key);

                if (cachedResult != null)
                {
                    return JsonConvert.DeserializeObject<T>(cachedResult);
                }

                var result = await factory();

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
                };

                await _cache.SetStringAsync(key, JsonConvert.SerializeObject(result), options);

                return result;
            }

            public async Task RemoveAsync(string key)
            {
                await _cache.RemoveAsync(key);
            }
        }
    }
}
