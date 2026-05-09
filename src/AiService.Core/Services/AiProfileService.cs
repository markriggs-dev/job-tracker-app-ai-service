using AiService.Core.DTOs;
using AiService.Core.Interfaces;
using AiService.Core.Models;

namespace AiService.Core.Services;

public class AiProfileService
{
    private readonly IAiProfileRepository _repo;

    public AiProfileService(IAiProfileRepository repo) => _repo = repo;

    public async Task<IEnumerable<AiProfileResponse>> GetAllAsync(string userId)
    {
        var profiles = await _repo.GetAllByUserAsync(userId);
        return profiles.Select(MapToResponse);
    }

    public async Task<AiProfileResponse> CreateAsync(string userId, CreateAiProfileRequest request)
    {
        var profile = new AiProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Instructions = request.Instructions,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var created = await _repo.CreateAsync(profile);
        return MapToResponse(created);
    }

    public async Task<AiProfileResponse?> UpdateAsync(Guid id, string userId, UpdateAiProfileRequest request)
    {
        var updated = await _repo.UpdateAsync(id, userId, request.Name, request.Instructions);
        return updated is null ? null : MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, string userId) =>
        await _repo.DeleteAsync(id, userId);

    private static AiProfileResponse MapToResponse(AiProfile p) =>
        new(p.Id, p.Name, p.Instructions, p.CreatedAt, p.UpdatedAt);
}
