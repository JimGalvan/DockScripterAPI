using Amazon.SQS.Model;

namespace DockScripter.Services.Interfaces;

public interface IScriptTriggerService
{
    Task<SendMessageResponse> SendScriptTriggerAsync(Guid scriptId, Dictionary<string, string> message);
}