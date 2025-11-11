using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WebApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        
        var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5119";
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    private void SetAuthorizationHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            // Remove existing auth header first to avoid duplicates
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    // Get single item - returns ApiResponse<T>
    public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.GetAsync(endpoint);
        
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ApiResponse<T>>(content);
    }

    // Get list - returns ApiResponse<List<T>>
    public async Task<ApiResponse<List<T>>?> GetListAsync<T>(string endpoint)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.GetAsync(endpoint);
        
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ApiResponse<List<T>>>(content);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
    {
        SetAuthorizationHeader();
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _httpClient.PostAsync(endpoint, content);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
    {
        SetAuthorizationHeader();
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _httpClient.PutAsync(endpoint, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        SetAuthorizationHeader();
        return await _httpClient.DeleteAsync(endpoint);
    }
}

// API Response model matching backend
public class ApiResponse<T>
{
    [JsonProperty("message")]
    public string Message { get; set; } = "";

    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("data")]
    public T? Data { get; set; }
}
