using MerkleFileServer.Infrastructure.Storage;
using Microsoft.Extensions.Caching.Memory;

namespace MerkleFileServer.Infrastructure.Storage.Memory
{
    public class MemoryStorage : IStorage
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryStorageOptions _options;

        public MemoryStorage(IMemoryCache cache, MemoryStorageOptions options)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Store<T>(string key, T data)
        {
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(_options.AbsoluteExpirationInMinutes));

            _cache.Set(key, data, options);
        }

        public object? Read(string key)
        {
            if (!_cache.TryGetValue(key, out object? cachedData))
            {
                return null;
            }
            else
            {
                return cachedData;
            }
        }
    }
}
