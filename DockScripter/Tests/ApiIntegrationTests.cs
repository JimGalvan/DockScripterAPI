using System;
using System.Threading.Tasks;
using Xunit;

public class ApiIntegrationTests
{
    private readonly AuthTestHelper _authHelper;
    private readonly ScriptTestHelper _scriptHelper;

    public ApiIntegrationTests()
    {
        var client = new ApiTestClient("http://localhost:5262/api/v1/");
        _authHelper = new AuthTestHelper(client);
        _scriptHelper = new ScriptTestHelper(client);
    }

    [Fact]
    public async Task HappyPathTestCase()
    {
        // 1. Register and Authenticate
        var authToken = await _authHelper.RegisterAndAuthenticateAsync();

        // 2. Create a Script
        var scriptId = await _scriptHelper.CreateScriptAsync(authToken);

        // 3. Upload Script Files
        await _scriptHelper.UploadScriptFileAsync(authToken, scriptId, "test_script.py", "print('Hello from script')");

        // 4. Execute the Script
        var executionResult = await _scriptHelper.ExecuteScriptAsync(authToken, scriptId);

        // 5. Assert Execution Results
        Assert.NotNull(executionResult);
        Assert.Equal("Success", executionResult.Status);
        Assert.NotNull(executionResult.ErrorOutputFilePath);
    }
}