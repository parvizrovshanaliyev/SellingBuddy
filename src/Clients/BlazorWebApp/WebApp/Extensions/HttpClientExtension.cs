using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebApp.Extensions;

public static class HttpClientExtension
{
    public static async Task<TResult> PostGetResponseAsync<TResult, TValue>(this HttpClient httpClient, string url,
        TValue value)
    {
        var httpResponse = await httpClient.PostAsJsonAsync(url, value);
        
        if(httpResponse.IsSuccessStatusCode)
        {
            var response = await httpResponse.Content.ReadFromJsonAsync<TResult>();
            return  response;
        }

        return  default;
    }
    
    public static async Task PostAsync<TValue>(this HttpClient httpClient, string url, TValue value)
    {
        await httpClient.PostAsJsonAsync(url, value);
    }
    
    public static async Task<TResult> GetResponseAsync<TResult>(this HttpClient httpClient, string url)
    {
        return await httpClient.GetFromJsonAsync<TResult>(url);
    }
}