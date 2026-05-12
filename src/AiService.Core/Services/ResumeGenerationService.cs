using System.Text;
using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using AiService.Core.Clients;
using AiService.Core.DTOs;
using AiService.Core.Interfaces;
using AiService.Core.Models;

namespace AiService.Core.Services;

public class ResumeGenerationService
{
    private readonly IGeneratedResumeRepository _repo;
    private readonly IAiProfileRepository _aiProfileRepo;
    private readonly IStorageService _storage;
    private readonly JobServiceClient _jobClient;
    private readonly ExperienceServiceClient _experienceClient;
    private readonly AnthropicClient _anthropic;

    public ResumeGenerationService(
        IGeneratedResumeRepository repo,
        IAiProfileRepository aiProfileRepo,
        IStorageService storage,
        JobServiceClient jobClient,
        ExperienceServiceClient experienceClient,
        AnthropicClient anthropic)
    {
        _repo = repo;
        _aiProfileRepo = aiProfileRepo;
        _storage = storage;
        _jobClient = jobClient;
        _experienceClient = experienceClient;
        _anthropic = anthropic;
    }

    public async Task<IEnumerable<GeneratedResumeResponse>> GetAllAsync(Guid jobId, string userId)
    {
        var resumes = await _repo.GetAllByJobAsync(jobId, userId);
        return resumes.Select(MapToResponse);
    }

    public async Task<GeneratedResumeResponse> GenerateAsync(
        Guid jobId, string userId, GenerateResumeRequest request, string bearerToken)
    {
        var aiProfile = await _aiProfileRepo.GetByIdAsync(request.AiProfileId, userId)
            ?? throw new InvalidOperationException("AI profile not found.");

        var jobDescription = await _jobClient.GetJobDescriptionAsync(jobId, bearerToken);
        if (string.IsNullOrWhiteSpace(jobDescription))
            throw new InvalidOperationException("Job description is empty. Add a job description before generating a resume.");

        var experienceContent = await _experienceClient.GetProfileContentAsync(request.ExperienceProfileId, bearerToken);
        if (string.IsNullOrWhiteSpace(experienceContent))
            throw new InvalidOperationException("Experience profile content could not be retrieved.");

        var prompt = BuildPrompt(jobDescription, experienceContent, aiProfile.Instructions);

        var messageRequest = new MessageParameters
        {
            Model = AnthropicModels.Claude35Sonnet,
            MaxTokens = 4096,
            Messages =
            [
                new Message { Role = RoleType.User, Content = [new TextContent { Text = prompt }] }
            ]
        };

        var result = await _anthropic.Messages.GetClaudeMessageAsync(messageRequest);
        var generatedText = result.Content.OfType<TextContent>().FirstOrDefault()?.Text
            ?? throw new InvalidOperationException("No content returned from Claude.");

        var resumeId = Guid.NewGuid();
        var fileName = $"resume_{jobId:N}_{resumeId:N}.md";
        var storageKey = $"{userId}/{jobId}/{resumeId}/{fileName}";

        var bytes = Encoding.UTF8.GetBytes(generatedText);
        using var stream = new MemoryStream(bytes);
        await _storage.UploadAsync(storageKey, stream, bytes.Length, "text/markdown");

        var record = new GeneratedResume
        {
            Id = resumeId,
            UserId = userId,
            JobRequisitionId = jobId,
            ExperienceProfileId = request.ExperienceProfileId,
            AiProfileId = request.AiProfileId,
            FileName = fileName,
            StorageKey = storageKey,
            FileSizeBytes = bytes.Length,
            GeneratedAt = DateTimeOffset.UtcNow
        };

        var created = await _repo.CreateAsync(record);
        return MapToResponse(created);
    }

    public async Task<(Stream Stream, string FileName)?> DownloadAsync(Guid id, string userId)
    {
        var record = await _repo.GetByIdAsync(id, userId);
        if (record is null) return null;
        var stream = await _storage.DownloadAsync(record.StorageKey);
        return (stream, record.FileName);
    }

    public async Task<bool> DeleteAsync(Guid id, string userId)
    {
        var record = await _repo.GetByIdAsync(id, userId);
        if (record is null) return false;
        await _storage.DeleteAsync(record.StorageKey);
        return await _repo.DeleteAsync(id, userId);
    }

    public async Task<BuildPromptResponse> BuildPromptAsync(
        Guid jobId, string userId, Guid experienceProfileId, Guid aiProfileId, string bearerToken)
    {
        var aiProfile = await _aiProfileRepo.GetByIdAsync(aiProfileId, userId)
            ?? throw new InvalidOperationException("AI profile not found.");

        var jobDescription = await _jobClient.GetJobDescriptionAsync(jobId, bearerToken);
        var experienceContent = await _experienceClient.GetProfileContentAsync(experienceProfileId, bearerToken);

        var prompt = BuildPrompt(
            string.IsNullOrWhiteSpace(jobDescription) ? "(No job description provided)" : jobDescription,
            experienceContent,
            aiProfile.Instructions);

        return new BuildPromptResponse(prompt, ExperienceEmbedded: !string.IsNullOrWhiteSpace(experienceContent));
    }

    private static string BuildPrompt(string jobDescription, string? experienceContent, string userInstructions)
    {
        var experienceSection = string.IsNullOrWhiteSpace(experienceContent)
            ? "[Attach your experience document to this conversation — PDF and DOCX files require manual attachment]"
            : experienceContent;

        return $"""
            You are an expert resume writer. Your task is to generate a tailored, ATS-optimized resume.

            ## Instructions
            {userInstructions}

            ## Job Description
            {jobDescription}

            ## Experience Document
            {experienceSection}

            ## Output
            Generate a complete, tailored resume in Markdown format. Include sections for Summary, Experience, Skills, and Education as applicable based on the instructions and experience document above.
            """;
    }

    private static string FormatFileSize(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _ => $"{bytes / (1024.0 * 1024):F1} MB"
    };

    private GeneratedResumeResponse MapToResponse(GeneratedResume r) => new(
        r.Id, r.JobRequisitionId, r.ExperienceProfileId, r.AiProfileId,
        r.FileName, FormatFileSize(r.FileSizeBytes), r.GeneratedAt);
}
