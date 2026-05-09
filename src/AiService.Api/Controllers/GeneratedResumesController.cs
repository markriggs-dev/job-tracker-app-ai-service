using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiService.Core.DTOs;
using AiService.Core.Services;

namespace AiService.Api.Controllers;

[ApiController]
[Route("api/jobs/{jobId:guid}/generated-resumes")]
[Authorize]
public class GeneratedResumesController : ControllerBase
{
    private readonly ResumeGenerationService _service;

    public GeneratedResumesController(ResumeGenerationService service) => _service = service;

    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token.");

    private string BearerToken =>
        Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid jobId)
    {
        var resumes = await _service.GetAllAsync(jobId, UserId);
        return Ok(resumes);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(Guid jobId, [FromBody] GenerateResumeRequest request)
    {
        try
        {
            var result = await _service.GenerateAsync(jobId, UserId, request, BearerToken);
            return CreatedAtAction(nameof(GetAll), new { jobId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid jobId, Guid id)
    {
        var result = await _service.DownloadAsync(id, UserId);
        if (result is null) return NotFound();
        return File(result.Value.Stream, "text/markdown", result.Value.FileName);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid jobId, Guid id)
    {
        var deleted = await _service.DeleteAsync(id, UserId);
        return deleted ? NoContent() : NotFound();
    }
}
