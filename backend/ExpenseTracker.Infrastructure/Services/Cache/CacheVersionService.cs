using ExpenseTracker.Application.Common.Caching;
using ExpenseTracker.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ExpenseTracker.Infrastructure.Services.Cache;
public class CacheVersionService : ICacheVersionService
{
    private readonly IMemoryCache _cache;

    public CacheVersionService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public int GetVersion(string cacheGroup, string userId)
    {
        var versionKey = CacheKeys.CacheVersion(cacheGroup, userId);

        if (!_cache.TryGetValue(versionKey, out int version))
        {
            version = 1;
            _cache.Set(versionKey, version);
        }

        return version;
    }

    public void IncrementVersion(string cacheGroup, string userId)
    {
        var version = GetVersion(cacheGroup, userId);

        _cache.Set(CacheKeys.CacheVersion(cacheGroup, userId), version + 1);
    }
}