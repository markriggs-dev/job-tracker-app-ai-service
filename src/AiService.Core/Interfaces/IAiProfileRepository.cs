using AiService.Core.Models;

namespace AiService.Core.Interfaces;

public interface IAiProfileRepository
{
    Task<IEnumerable<AiProfile>> GetAllByUserAsync(string userId);
    Task<AiProfile?> GetByIdAsync(Guid id, string userId);
    Task<AiProfile> CreateAsync(AiProfile profile);
    Task<AiProfile?> UpdateAsync(Guid id, string userId, string name, string instructions);
    Task<bool> DeleteAsync(Guid id, string userId);
}
