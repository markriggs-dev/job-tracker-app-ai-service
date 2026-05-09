namespace AiService.Infrastructure.Storage;

public class StorageSettings
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = "generated-resumes";
    public bool UseSSL { get; set; } = false;
}
