using Amazon.SecretsManager.Model;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace DockScripter.Core.Aws;

using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

public class AwsSecretsManagerClient
{
    private readonly IAmazonSecretsManager _client;

    public AwsSecretsManagerClient()
    {
        _client = new AmazonSecretsManagerClient();
    }

    public async Task<string> GetSecret(string secretName)
    {
        try
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT"
            };

            var response = await _client.GetSecretValueAsync(request);

            if (!string.IsNullOrEmpty(response.SecretString))
            {
                return response.SecretString;
            }

            else if (response.SecretBinary != null)
            {
                using var reader = new StreamReader(response.SecretBinary);
                var decodedBinarySecret = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(reader.ReadToEnd()));
                return decodedBinarySecret;
            }

            return string.Empty;
        }
        catch (AmazonSecretsManagerException e)
        {
            Console.WriteLine($"Error retrieving secret: {e.Message}");
            throw;
        }
    }
}