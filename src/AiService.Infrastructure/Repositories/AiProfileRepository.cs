using Microsoft.EntityFrameworkCore;
using AiService.Core.Interfaces;
using AiService.Core.Models;
using AiService.Infrastructure.Data;

namespace AiService.Infrastructure.Repositories;

public class AiProfileRepository : IAiProfileRepository
{
    private readonly AiServiceDbContext _db;

    public AiProfileRepository(AiServiceDbContext db) => _db = db;

    public async Task<IEnumerable<AiProfile>> GetAllByUserAsync(string userId) =>
        await _db.AiProfiles
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();

    public async Task<AiProfile?> GetByIdAsync(Guid id, string userId) =>
        await _db.AiProfiles.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

    public async Task<AiProfile> CreateAsync(AiProfile profile)
    {
        _db.AiProfiles.Add(profile);
        await _db.SaveChangesAsync();
        return profile;
    }

    public async Task<AiProfile?> UpdateAsync(Guid id, string userId, string name, string instructions)
    {
        var profile = await _db.AiProfiles.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (profile is null) return null;
        profile.Name = name;
        profile.Instructions = instructions;
        profile.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
        return profile;
    }

    public async Task<bool> DeleteAsync(Guid id, string userId)
    {
        var profile = await _db.AiProfiles.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (profile is null) return false;
        _db.AiProfiles.Remove(profile);
        await _db.SaveChangesAsync();
        return true;
    }
}
