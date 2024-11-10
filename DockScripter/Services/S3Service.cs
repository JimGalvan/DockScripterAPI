using Amazon.S3;
using Amazon.S3.Model;
using DockScripter.Services.Interfaces;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration["S3BucketName"]!;

        if (string.IsNullOrEmpty(_bucketName))
        {
            throw new Exception($"S3 bucket name is not configured. Bucket: '{_bucketName}' is invalid.");
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = "application/octet-stream"
        };

        var response = await _s3Client.PutObjectAsync(request);

        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception("Failed to upload file to S3");
        }

        return fileName;
    }

    public async Task<Stream> DownloadFileAsync(string fileKey)
    {
        var response = await _s3Client.GetObjectAsync(_bucketName, fileKey);
        return response.ResponseStream;
    }
}