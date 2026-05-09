using System.Net.Http.Json;

namespace AiService.Core.Clients;

public class JobServiceClient
{
    private readonly HttpClient _http;

    public JobServiceClient(HttpClient http) => _http = http;

    public async Task<string?> GetJobDescriptionAsync(Guid jobId, string bearerToken)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"/api/jobs/{jobId}");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        var response = await _http.SendAsync(req);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadFromJsonAsync<JobDto>();
        return json?.JobDescription;
    }

    private record JobDto(string? JobDescription);
}
