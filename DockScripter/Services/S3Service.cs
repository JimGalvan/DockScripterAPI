using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DockScripter.Core.Aws;
using DockScripter.Services.Interfaces;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Service(IConfiguration configuration)
    {
        var environment = configuration["Environment"];
        var awsRegion = configuration["AWS:Region"]
                        ?? throw new Exception("AWS region is not configured properly.");

        if (environment == "Development")
        {
            var awsAccessKeyId = configuration["AWS:AccessKeyId"];
            var awsSecretAccessKey = configuration["AWS:SecretAccessKey"];
            _bucketName = configuration["AWS:S3BucketName"]
                          ?? throw new Exception("S3 bucket name is not configured.");

            if (string.IsNullOrEmpty(awsAccessKeyId) || string.IsNullOrEmpty(awsSecretAccessKey))
            {
                throw new Exception("AWS credentials are not configured properly for development.");
            }

            _s3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey,
                RegionEndpoint.GetBySystemName(awsRegion));
        }
        else
        {
            _s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(awsRegion));
            var secretsClient = new AwsSecretsManagerClient();
            var secretValue = secretsClient.GetSecret("DockScripterS3BucketName");
            _bucketName = secretValue.ToString() ?? throw new Exception("S3 bucket name is not configured.");
        }
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