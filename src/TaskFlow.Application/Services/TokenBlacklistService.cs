using System.Collections.Concurrent;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Application.Services;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

    public Task BlacklistTokenAsync(string token, DateTime expiryDate)
    {
        _blacklistedTokens.TryAdd(token, expiryDate);
        return Task.CompletedTask;
    }

    public Task<bool> IsTokenBlacklistedAsync(string token)
    {
        if (_blacklistedTokens.TryGetValue(token, out var expiryDate))
        {
            if (expiryDate > DateTime.UtcNow)
                return Task.FromResult(true);
            
            _blacklistedTokens.TryRemove(token, out _);
        }
        return Task.FromResult(false);
    }
}
