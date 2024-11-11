using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DockScripter.Tests;
using Xunit;

public class ApiIntegrationTests
{
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        { PropertyNameCaseInsensitive = true };

    public ApiIntegrationTests()
    {
        _client = new HttpClient
            { BaseAddress = new Uri("http://localhost:5262/api/v1/") };
        _client.Timeout = Timeout.InfiniteTimeSpan;
    }

    [Fact]
    public async Task HappyPathTestCase()
    {
        // 1. Register and Authenticate
        var authToken = await RegisterAndAuthenticateUserAsync();

        // 2. Create a Script
        var scriptId = await CreateScriptAsync(authToken);

        // 3. Upload Script Files
        await UploadScriptFileAsync(authToken, scriptId, "test_script.py", "print('Hello from script')");

        // 4. Execute the Script
        var executionResult = await ExecuteScriptAsync(authToken, scriptId);

        // 5. Retrieve and Assert Execution Results
        Assert.NotNull(executionResult);
        Assert.Equal("Success", executionResult.Status);
        Assert.Contains("Hello from script", executionResult.Output);
    }

    private async Task<string> RegisterAndAuthenticateUserAsync()
    {
        var registerPayload = new
        {
            FirstName = TestDataGenerator.GenerateFirstName(),
            LastName = TestDataGenerator.GenerateLastName(),
            Email = TestDataGenerator.GenerateEmail(),
            Password = "Password123!"
        };

        var registerResponse = await _client.PostAsync("auth/register",
            new StringContent(JsonSerializer.Serialize(registerPayload), Encoding.UTF8, "application/json"));
        registerResponse.EnsureSuccessStatusCode();

        var loginPayload = new { Email = registerPayload.Email, Password = "Password123!" };
        var loginResponse = await _client.PostAsync("auth/login",
            new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json"));
        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginData = JsonSerializer.Deserialize<AuthResponseDto>(loginContent, _jsonOptions);
        return loginData.Token;
    }

    private async Task<Guid> CreateScriptAsync(string authToken)
    {
        var scriptPayload = new
        {
            Name = TestDataGenerator.GenerateScriptName(),
            Description = TestDataGenerator.GenerateScriptDescription(),
            EntryFilePath = "test_script.py" // Specify the entry file path
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "script")
        {
            Content = new StringContent(JsonSerializer.Serialize(scriptPayload), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var scriptData = JsonSerializer.Deserialize<ScriptResponseDto>(content, _jsonOptions);
        return scriptData.Id;
    }

    private async Task UploadScriptFileAsync(string authToken, Guid scriptId, string fileName, string fileContent)
    {
        using var fileContentStream = new ByteArrayContent(Encoding.UTF8.GetBytes(fileContent));
        fileContentStream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");

        var form = new MultipartFormDataContent
        {
            { fileContentStream, "file", fileName }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"script/{scriptId}/upload")
        {
            Content = form
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private async Task<ExecutionResultResponseDto> ExecuteScriptAsync(string authToken, Guid scriptId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"script/execute/{scriptId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ExecutionResultResponseDto>(content, _jsonOptions);
    }

    private class AuthResponseDto
    {
        public string Token { get; set; }
    }

    private class ScriptResponseDto
    {
        public Guid Id { get; set; }
    }

    private class ExecutionResultResponseDto
    {
        public Guid Id { get; set; }
        public string Output { get; set; }
        public string ErrorOutput { get; set; }
        public string Status { get; set; }
        public DateTime ExecutedAt { get; set; }
    }
}
