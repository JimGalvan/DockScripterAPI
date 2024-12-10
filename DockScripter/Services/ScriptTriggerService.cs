using DockScripter.Services.Interfaces;

namespace DockScripter.Services;

using Amazon.SQS;
using Amazon.SQS.Model;

public class ScriptTriggerService : IScriptTriggerService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;

    public ScriptTriggerService(IConfiguration configuration)
    {
        var environment = configuration["Environment"];

        if (environment == "Development")
        {
            var region = configuration["AWS:Region"];
            _sqsClient = new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(region));
            _queueUrl = configuration["AWS:SQSQueueURL"];

            if (_sqsClient == null)
            {
                throw new Exception("SQS client is not configured.");
            }
        }
        else
        {
        }
    }

    public async Task<SendMessageResponse> SendScriptTriggerAsync(Guid scriptId,
        Dictionary<string, string> parameters)
    {
        var messageBody = System.Text.Json.JsonSerializer.Serialize(new
        {
            scriptId = scriptId,
            Parameters = parameters
        });

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = messageBody
        };
        return await _sqsClient.SendMessageAsync(sendMessageRequest);
    }
}