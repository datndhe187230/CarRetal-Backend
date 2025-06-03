namespace CarRental_BE.Services
{
    public interface IRedisService
    {
        Task SaveTokenAsync(string prefix, string postfixm, string token, TimeSpan ttl);

        Task<string?> GetTokenAsync(string prefix, string postfix);

        Task DeleteTokenAsync(string prefix, string postfix);
    }
}
