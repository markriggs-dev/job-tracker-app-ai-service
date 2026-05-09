namespace AiService.Core.Models;

public class GeneratedResume
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid JobRequisitionId { get; set; }
    public Guid ExperienceProfileId { get; set; }
    public Guid AiProfileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
}
