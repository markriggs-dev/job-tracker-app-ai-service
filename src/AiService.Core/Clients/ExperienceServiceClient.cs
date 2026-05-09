namespace AiService.Core.Clients;

public class ExperienceServiceClient
{
    private readonly HttpClient _http;

    public ExperienceServiceClient(HttpClient http) => _http = http;

    public async Task<string?> GetProfileContentAsync(Guid profileId, string bearerToken)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"/api/experience-profiles/{profileId}/download");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        var response = await _http.SendAsync(req);
        if (!response.IsSuccessStatusCode) return null;

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
        if (!contentType.StartsWith("text/")) return null;

        var bytes = await response.Content.ReadAsByteArrayAsync();
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
