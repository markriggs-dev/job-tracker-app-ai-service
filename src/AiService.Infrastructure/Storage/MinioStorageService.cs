using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using AiService.Core.Interfaces;

namespace AiService.Infrastructure.Storage;

public class MinioStorageService : IStorageService
{
    private readonly IMinioClient _minio;
    private readonly string _bucket;

    public MinioStorageService(IMinioClient minio, IOptions<StorageSettings> settings)
    {
        _minio = minio;
        _bucket = settings.Value.BucketName;
    }

    public async Task EnsureBucketExistsAsync(CancellationToken ct = default)
    {
        var exists = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket), ct);
        if (!exists)
            await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket), ct);
    }

    public async Task UploadAsync(string key, Stream stream, long size, string contentType, CancellationToken ct = default)
    {
        await _minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucket).WithObject(key)
            .WithStreamData(stream).WithObjectSize(size)
            .WithContentType(contentType), ct);
    }

    public async Task<Stream> DownloadAsync(string key, CancellationToken ct = default)
    {
        var ms = new MemoryStream();
        await _minio.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucket).WithObject(key)
            .WithCallbackStream(s => s.CopyTo(ms)), ct);
        ms.Position = 0;
        return ms;
    }

    public async Task DeleteAsync(string key, CancellationToken ct = default)
    {
        await _minio.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucket).WithObject(key), ct);
    }
}
