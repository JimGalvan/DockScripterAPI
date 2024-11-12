namespace DockScripter.Services.Interfaces;

public interface IS3Service
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName);
    Task<Stream> DownloadFileAsync(string fileKey);

    Task<string> UploadLogFileAsync(string logContent, string fileName);
}