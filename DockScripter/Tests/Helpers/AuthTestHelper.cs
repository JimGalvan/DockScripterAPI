using System.Threading.Tasks;
using DockScripter.Tests;
using DockScripter.Tests.Helpers.TestDtos;

public class AuthTestHelper
{
    private readonly ApiTestClient _client;

    public AuthTestHelper(ApiTestClient client)
    {
        _client = client;
    }

    public async Task<string> RegisterAndAuthenticateAsync()
    {
        var password = "Password123!";

        var registerPayload = new
        {
            FirstName = TestDataGenerator.GenerateFirstName(),
            LastName = TestDataGenerator.GenerateLastName(),
            Email = TestDataGenerator.GenerateEmail(),
            Password = password
        };

        await _client.PostAsync("auth/register", registerPayload);

        var loginPayload = new { Email = registerPayload.Email, Password = password };
        var loginResponse = await _client.PostAsync<AuthResponseDto>("auth/login", loginPayload);

        return loginResponse.Token;
    }
}