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

        // 3. Initialize Environment
        var environmentId = await InitializeEnvironmentAsync(authToken);

        // 4. Execute the Script
        var executionResult = await ExecuteScriptAsync(authToken, scriptId);

        // 5. Retrieve and Assert Execution Results
        Assert.NotNull(executionResult);
        Assert.Equal("Success", executionResult.Status);
        Assert.Contains("Expected output", executionResult.Output);
    }

    private async Task<string> RegisterAndAuthenticateUserAsync()
    {
        // Register user
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

        // Authenticate user and get token
        var loginPayload = new { Email = "testuser@example.com", Password = "Password123!" };
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
            FilePath = "/scripts/test_script.py",
            Language = "Python"
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

    private async Task<Guid> InitializeEnvironmentAsync(string authToken)
    {
        var environmentPayload = new { EnvironmentName = TestDataGenerator.GenerateEnvironmentName() };

        var request = new HttpRequestMessage(HttpMethod.Post, "environment")
        {
            Content = new StringContent(JsonSerializer.Serialize(environmentPayload), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var environmentData = JsonSerializer.Deserialize<EnvironmentResponseDto>(content, _jsonOptions);
        return environmentData.Id;
    }

    private async Task<ExecutionResultResponseDto> ExecuteScriptAsync(string authToken, Guid scriptId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"Script/execute/{scriptId}");
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

    private class EnvironmentResponseDto
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