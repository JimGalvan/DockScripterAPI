using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DockScripter.Services.Interfaces;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Service(IConfiguration configuration)
    {
        var awsAccessKeyId = configuration["AWS:AccessKeyId"];
        var awsSecretAccessKey = configuration["AWS:SecretAccessKey"];
        var awsRegion = configuration["AWS:Region"];

        if (string.IsNullOrEmpty(awsAccessKeyId) || string.IsNullOrEmpty(awsSecretAccessKey) ||
            string.IsNullOrEmpty(awsRegion))
        {
            throw new Exception("AWS credentials or region are not configured properly.");
        }

        _s3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(awsRegion));
        _bucketName = configuration["S3BucketName"] ?? throw new Exception("S3 bucket name is not configured.");
    }

    public async Task<PutObjectResponse> UploadFileAsync(Stream fileStream, string fileName)
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

        return response;
    }

    public async Task<PutObjectResponse> UploadLogFileAsync(string logContent, string fileName)
    {
        var logStream = new MemoryStream(Encoding.UTF8.GetBytes(logContent));
        return await UploadFileAsync(logStream, fileName);
    }

    public async Task<Stream> DownloadFileAsync(string fileKey)
    {
        var response = await _s3Client.GetObjectAsync(_bucketName, fileKey);
        return response.ResponseStream;
    }
}