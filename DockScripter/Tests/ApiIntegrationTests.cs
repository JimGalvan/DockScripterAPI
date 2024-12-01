using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DockScripter.Domain.Entities;
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
    public async Task TestCreateScript()
    {
        // 1. Register and Authenticate
        var authToken = await RegisterAndAuthenticateUserAsync();
        var scriptPayload = new ScriptPayload
        {
            Name = TestDataGenerator.GenerateScriptName(),
            Description = TestDataGenerator.GenerateScriptDescription(),
            EntryFilePath = "test_script.py",
            Language = "Python",
            DockerImage = "python:3.8",
            Files = new[]
            {
                new ScriptFile { Name = "test_script.py", Content = "print('Hello from script')" }
            }
        };

        // 2. Create a Script
        var scriptData = await CreateScriptAsync(authToken, scriptPayload);

        // 3. Assert the response
        Assert.NotNull(scriptData);
        Assert.Equal(scriptPayload.Name, scriptData.Name);
        Assert.Equal(scriptPayload.EntryFilePath, scriptData.EntryFilePath);
        Assert.Equal(scriptPayload.Description, scriptData.Description);
        Assert.Equal(scriptPayload.Language, scriptData.Language);
        Assert.NotEqual(Guid.Empty, scriptData.Id);
    }

    [Fact]
    public async Task TestUserCanExecuteScript()
    {
        var scriptPayload = new ScriptPayload
        {
            Name = TestDataGenerator.GenerateScriptName(),
            Description = TestDataGenerator.GenerateScriptDescription(),
            EntryFilePath = "test_script.py",
            Language = "Python",
            DockerImage = "python:3.8",
            Files = new[]
            {
                new ScriptFile { Name = "test_script.py", Content = "print('Hello from script')" }
            }
        };

        // 1. Register and Authenticate
        var authToken = await RegisterAndAuthenticateUserAsync();

        // 2. Create a Script
        var script = await CreateScriptAsync(authToken, scriptPayload);
        var scriptId = script.Id;

        // 3. Execute the Script
        var executionResult = await ExecuteScriptAsync(authToken, scriptId);

        // 4. Retrieve and Assert Execution Results
        Assert.NotNull(executionResult);
        Assert.Equal("Success", executionResult.Status);
        Assert.NotNull(executionResult.ErrorOutputFilePath);
    }

    [Fact]
    public async Task TestUserCanDeleteScript()
    {
        var scriptPayload = new ScriptPayload
        {
            Name = TestDataGenerator.GenerateScriptName(),
            Description = TestDataGenerator.GenerateScriptDescription(),
            EntryFilePath = "test_script.py",
            Language = "Python",
            DockerImage = "python:3.8",
            Files = new[]
            {
                new ScriptFile { Name = "test_script.py", Content = "print('Hello from script')" }
            }
        };

        // 1. Register and Authenticate
        var authToken = await RegisterAndAuthenticateUserAsync();

        // 2. Create a Script
        var script = await CreateScriptAsync(authToken, scriptPayload);
        var scriptId = script.Id;

        // 3. Delete the Script
        var request = new HttpRequestMessage(HttpMethod.Delete, $"script/{scriptId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // 4. Retrieve and Assert Script Deletion
        request = new HttpRequestMessage(HttpMethod.Get, $"script/{scriptId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var getResponse = await _client.SendAsync(request);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
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

    private async Task<ScriptResponseDto> CreateScriptAsync(string authToken, ScriptPayload scriptPayload)
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent(scriptPayload.Name), "Name" },
            { new StringContent(scriptPayload.Description), "Description" },
            { new StringContent(scriptPayload.EntryFilePath), "EntryFilePath" },
            { new StringContent(scriptPayload.Language), "Language" },
            { new StringContent(scriptPayload.DockerImage), "DockerImage" }
        };

        foreach (var file in scriptPayload.Files)
        {
            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(file.Content));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            formData.Add(fileContent, "Files", file.Name);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "script")
        {
            Content = formData
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var scriptData = JsonSerializer.Deserialize<ScriptResponseDto>(content, _jsonOptions);
        return scriptData;
    }

    private async Task UploadScriptFileAsync(string authToken, Guid scriptId, string fileName, string fileContent)
    {
        using var fileContentStream = new ByteArrayContent(Encoding.UTF8.GetBytes(fileContent));
        fileContentStream.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

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
        var request = new HttpRequestMessage(HttpMethod.Post, $"script/{scriptId}/execute");
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
        public string Name { get; set; }
        public string Description { get; set; }
        public string EntryFilePath { get; set; }
        public string Language { get; set; }
    }

    private class ExecutionResultResponseDto
    {
        public Guid Id { get; set; }
        public string? Output { get; set; }
        public string? ErrorOutput { get; set; }
        public string? OutputFilePath { get; set; }
        public string? ErrorOutputFilePath { get; set; }
        public DateTime ExecutedAt { get; set; }
        public string? Status { get; set; }
    }

    public class ScriptPayload
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string EntryFilePath { get; set; }
        public string Language { get; set; }
        public string DockerImage { get; set; }
        public ScriptFile[] Files { get; set; }
    }

    public class ScriptFile
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
}