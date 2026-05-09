using Microsoft.EntityFrameworkCore;
using AiService.Core.Interfaces;
using AiService.Core.Models;
using AiService.Infrastructure.Data;

namespace AiService.Infrastructure.Repositories;

public class GeneratedResumeRepository : IGeneratedResumeRepository
{
    private readonly AiServiceDbContext _db;

    public GeneratedResumeRepository(AiServiceDbContext db) => _db = db;

    public async Task<IEnumerable<GeneratedResume>> GetAllByJobAsync(Guid jobRequisitionId, string userId) =>
        await _db.GeneratedResumes
            .Where(r => r.JobRequisitionId == jobRequisitionId && r.UserId == userId)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();

    public async Task<GeneratedResume?> GetByIdAsync(Guid id, string userId) =>
        await _db.GeneratedResumes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

    public async Task<GeneratedResume> CreateAsync(GeneratedResume resume)
    {
        _db.GeneratedResumes.Add(resume);
        await _db.SaveChangesAsync();
        return resume;
    }

    public async Task<bool> DeleteAsync(Guid id, string userId)
    {
        var resume = await _db.GeneratedResumes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (resume is null) return false;
        _db.GeneratedResumes.Remove(resume);
        await _db.SaveChangesAsync();
        return true;
    }
}
