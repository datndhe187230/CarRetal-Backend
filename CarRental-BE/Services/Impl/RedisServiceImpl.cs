
using StackExchange.Redis;

namespace CarRental_BE.Services.Impl
{
    public class RedisServiceImpl : IRedisService
    {
        private readonly IDatabase _db;

        public RedisServiceImpl(IConnectionMultiplexer connectionMultiplexer)
        {
            _db = connectionMultiplexer.GetDatabase();
        }

        public async Task DeleteTokenAsync(string prefix, string postfix)
        {
            String RedisKey = prefix + ":" + postfix;
            await _db.KeyDeleteAsync(RedisKey);
        }

        public async Task<string?> GetTokenAsync(string prefix, string postfix)
        {
            String RedisKey = prefix + ":" + postfix;
            return await _db.StringGetAsync(RedisKey);
        }

        public async Task SaveTokenAsync(string prefix, string postfix, string token, TimeSpan ttl)
        {
            String RedisKey = prefix + ":" + postfix;
            await _db.StringSetAsync(RedisKey, token, ttl);
        }
    }
}
