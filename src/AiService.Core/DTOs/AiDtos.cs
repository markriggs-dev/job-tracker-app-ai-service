namespace AiService.Core.DTOs;

public record AiProfileResponse(
    Guid Id,
    string Name,
    string Instructions,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

public record CreateAiProfileRequest(string Name, string Instructions);

public record UpdateAiProfileRequest(string Name, string Instructions);

public record GeneratedResumeResponse(
    Guid Id,
    Guid JobRequisitionId,
    Guid ExperienceProfileId,
    Guid AiProfileId,
    string FileName,
    string FileSizeDisplay,
    DateTimeOffset GeneratedAt
);

public record GenerateResumeRequest(Guid ExperienceProfileId, Guid AiProfileId);
