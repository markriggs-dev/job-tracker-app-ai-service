namespace AiService.Core.Interfaces;

public interface IStorageService
{
    Task EnsureBucketExistsAsync(CancellationToken ct = default);
    Task UploadAsync(string key, Stream stream, long size, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string key, CancellationToken ct = default);
    Task DeleteAsync(string key, CancellationToken ct = default);
}
