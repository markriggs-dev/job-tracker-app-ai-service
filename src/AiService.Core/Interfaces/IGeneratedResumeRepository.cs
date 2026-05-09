using AiService.Core.Models;

namespace AiService.Core.Interfaces;

public interface IGeneratedResumeRepository
{
    Task<IEnumerable<GeneratedResume>> GetAllByJobAsync(Guid jobRequisitionId, string userId);
    Task<GeneratedResume?> GetByIdAsync(Guid id, string userId);
    Task<GeneratedResume> CreateAsync(GeneratedResume resume);
    Task<bool> DeleteAsync(Guid id, string userId);
}
