using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ApiTestClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiTestClient(string baseUrl)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = Timeout.InfiniteTimeSpan
        };
    }

    public async Task<T> PostAsync<T>(string url, object payload, string? authToken = null)
    {
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var request = CreateRequest(HttpMethod.Post, url, content, authToken);

        using var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions)!;
    }

    public async Task PostAsync(string url, object payload, string? authToken = null)
    {
        await PostAsync<object>(url, payload, authToken);
    }

    public async Task PostFormAsync(string url, MultipartFormDataContent formData, string authToken)
    {
        var request = CreateRequest(HttpMethod.Post, url, formData, authToken);

        using var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url, HttpContent? content, string? authToken)
    {
        var request = new HttpRequestMessage(method, url)
        {
            Content = content
        };

        if (!string.IsNullOrEmpty(authToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

        return request;
    }
}