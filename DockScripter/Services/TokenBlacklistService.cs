using System.Collections.Concurrent;
using ParkOnyx.Services.Interfaces;

namespace ParkOnyx.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, bool> _blacklistedTokens = new();

        public void BlacklistToken(string token)
        {
            _blacklistedTokens[token] = true;
        }

        public bool IsTokenBlacklisted(string token)
        {
            return _blacklistedTokens.ContainsKey(token);
        }
    }
}