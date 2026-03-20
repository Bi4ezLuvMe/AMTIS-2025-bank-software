
using System.Text;
using System.Text.Json;

public class ApiClient
{
    private readonly HttpClient client;
    private readonly string baseUrl = "https://competition-engine-production.up.railway.app/";

    public ApiClient()
    {
        client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; BankModule/1.0)");
    }

    public async Task<HttpResponseMessage> PostAsync(string endpoint, object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PostAsync(baseUrl + endpoint, content);
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + endpoint);
        if (headers != null)
        {
            foreach (var h in headers)
                request.Headers.Add(h.Key, h.Value);
        }
        return await client.SendAsync(request);
    }

    public async Task<HttpResponseMessage> PatchAsync(string endpoint, object payload, Dictionary<string, string>? headers = null)
    {
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), baseUrl + endpoint)
        {
            Content = content
        };
        if (headers != null)
        {
            foreach (var h in headers)
                request.Headers.Add(h.Key, h.Value);
        }
        return await client.SendAsync(request);
    }

    public async Task<HttpResponseMessage> PutAsync(string endpoint, object payload, Dictionary<string, string>? headers = null)
    {
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Put, baseUrl + endpoint)
        {
            Content = content
        };
        if (headers != null)
        {
            foreach (var h in headers)
                request.Headers.Add(h.Key, h.Value);
        }
        return await client.SendAsync(request);
    }
}
