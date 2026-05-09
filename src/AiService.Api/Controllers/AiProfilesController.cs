using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiService.Core.DTOs;
using AiService.Core.Services;

namespace AiService.Api.Controllers;

[ApiController]
[Route("api/ai-profiles")]
[Authorize]
public class AiProfilesController : ControllerBase
{
    private readonly AiProfileService _service;

    public AiProfilesController(AiProfileService service) => _service = service;

    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token.");

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var profiles = await _service.GetAllAsync(UserId);
        return Ok(profiles);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAiProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Profile name is required." });
        if (string.IsNullOrWhiteSpace(request.Instructions))
            return BadRequest(new { error = "Instructions are required." });

        var result = await _service.CreateAsync(UserId, request);
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAiProfileRequest request)
    {
        var result = await _service.UpdateAsync(id, UserId, request);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id, UserId);
        return deleted ? NoContent() : NotFound();
    }
}
