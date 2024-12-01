using Amazon.S3.Model;

namespace DockScripter.Services.Interfaces;

public interface IS3Service
{
    Task<PutObjectResponse> UploadFileAsync(Stream fileStream, string fileName);
    Task<Stream> DownloadFileAsync(string fileKey);

    Task<PutObjectResponse> UploadLogFileAsync(string logContent, string fileName);
}