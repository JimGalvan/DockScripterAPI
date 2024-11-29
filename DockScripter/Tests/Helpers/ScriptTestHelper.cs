using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DockScripter.Domain.Dtos.Responses;
using DockScripter.Tests;

public class ScriptTestHelper
{
    private readonly ApiTestClient _client;

    public ScriptTestHelper(ApiTestClient client)
    {
        _client = client;
    }

    public async Task<Guid> CreateScriptAsync(string authToken)
    {
        var scriptPayload = new
        {
            Name = TestDataGenerator.GenerateScriptName(),
            Description = TestDataGenerator.GenerateScriptDescription(),
            EntryFilePath = "test_script.py",
            Language = "Python"
        };

        var response = await _client.PostAsync<ScriptResponseDto>("script", scriptPayload, authToken);
        return response.Id;
    }

    public async Task UploadScriptFileAsync(string authToken, Guid scriptId, string fileName, string fileContent)
    {
        using var fileContentStream = new ByteArrayContent(Encoding.UTF8.GetBytes(fileContent))
        {
            Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain") }
        };

        using var form = new MultipartFormDataContent
        {
            { fileContentStream, "file", fileName }
        };

        var url = $"script/{scriptId}/upload";
        await _client.PostFormAsync(url, form, authToken);
    }

    public async Task<ExecutionResultResponseDto> ExecuteScriptAsync(string authToken, Guid scriptId)
    {
        var url = $"script/execute/{scriptId}";
        return await _client.PostAsync<ExecutionResultResponseDto>(url, null, authToken);
    }
}