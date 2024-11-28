using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using WebApp.Domain.User;

namespace WebApp.Extensions;

public static class HttpClientExtension
{
    /// <summary>
    /// Common method to handle the HTTP response status and errors.
    /// </summary>
    /// <param name="httpResponse">The HTTP response.</param>
    /// <param name="url">The URL of the request.</param>
    /// <returns>Returns the error message or deserialized content if successful.</returns>
    private static async Task<string> HandleResponseAsync(HttpResponseMessage httpResponse, string url)
    {
        if (httpResponse.IsSuccessStatusCode)
        {
            return await httpResponse.Content.ReadAsStringAsync();
        }

        // Handle different HTTP response codes
        var errorMessage = await httpResponse.Content.ReadAsStringAsync();
        switch (httpResponse.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                throw new HttpRequestException($"Bad Request: {errorMessage} for URL: {url}");
            case HttpStatusCode.Unauthorized:
                throw new UnauthorizedAccessException($"Unauthorized: {errorMessage} for URL: {url}");
            case HttpStatusCode.Forbidden:
                throw new UnauthorizedAccessException($"Forbidden: {errorMessage} for URL: {url}");
            case HttpStatusCode.NotFound:
                throw new HttpRequestException($"Not Found: {errorMessage} for URL: {url}");
            case HttpStatusCode.InternalServerError:
                throw new HttpRequestException($"Server Error: {errorMessage} for URL: {url}");
            default:
                throw new HttpRequestException($"Request failed: {httpResponse.StatusCode} for URL: {url}. Details: {errorMessage}");
        }
    }

    /// <summary>
    /// Sends a POST request with a JSON body to the specified URL and returns a deserialized response of type TResponse if the request is successful.
    /// If the response is not successful, an exception is thrown with the error details.
    /// </summary>
    /// <typeparam name="TResponse">The type of the expected response.</typeparam>
    /// <typeparam name="TRequest">The type of the value being sent in the request body.</typeparam>
    /// <param name="httpClient">The HttpClient instance.</param>
    /// <param name="url">The URL to send the POST request to.</param>
    /// <param name="value">The value to be sent in the request body.</param>
    /// <returns>A task representing the asynchronous operation, with the deserialized result as the outcome.</returns>
    /// <exception cref="HttpRequestException">Thrown if the response is not successful or if an error occurs during the HTTP request.</exception>
    public static async Task<TResponse> PostGetResponseAsync<TResponse, TRequest>(
        this HttpClient httpClient,
        string url,
        TRequest value)
        where TResponse : ResponseBase, new()
        where TRequest : RequestBase
    {
        try
        {
            var httpResponse = await httpClient.PostAsJsonAsync(url, value);

            var resultContent = await HandleResponseAsync(httpResponse, url);

            // Deserialize the response content into TResponse with optional options
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Example for camel case
            };
            
            // Attempt to deserialize the response content into TResponse
            var response = JsonSerializer.Deserialize<TResponse>(resultContent ?? string.Empty, options);

            if (response == null)
            {
                throw new InvalidOperationException("Response content could not be deserialized.");
            }

            return response;
        }
        catch (HttpRequestException httpEx)
        {
            throw new HttpRequestException($"An error occurred while making the HTTP request to {url}.", httpEx);
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"An unexpected error occurred while making the HTTP request to {url}.", ex);
        }
    }

    /// <summary>
    /// Sends a POST request with a JSON body to the specified URL without expecting a response body.
    /// If the response is not successful, an exception is thrown with the error details.
    /// </summary>
    /// <typeparam name="TRequest">The type of the value being sent in the request body.</typeparam>
    /// <param name="httpClient">The HttpClient instance.</param>
    /// <param name="url">The URL to send the POST request to.</param>
    /// <param name="value">The value to be sent in the request body.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown if the response is not successful or if an error occurs during the HTTP request.</exception>
    public static async Task PostAsync<TRequest>(this HttpClient httpClient, string url, TRequest value)
        where TRequest : RequestBase
    {
        try
        {
            var httpResponse = await httpClient.PostAsJsonAsync(url, value);
            await HandleResponseAsync(httpResponse, url);
        }
        catch (HttpRequestException httpEx)
        {
            throw new HttpRequestException($"An error occurred while making the HTTP request to {url}.", httpEx);
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"An unexpected error occurred while making the HTTP request to {url}.", ex);
        }
    }

    /// <summary>
    /// Sends a GET request to the specified URL and returns a deserialized response of type TResult.
    /// Handles specific HTTP status codes and provides better exception handling.
    /// </summary>
    /// <typeparam name="TResponse">The type of the expected response.</typeparam>
    /// <param name="httpClient">The HttpClient instance.</param>
    /// <param name="url">The URL to send the GET request to.</param>
    /// <returns>A task representing the asynchronous operation, with the deserialized result as the outcome.</returns>
    /// <exception cref="HttpRequestException">Thrown if an error occurs during the HTTP request.</exception>
    public static async Task<TResponse> GetResponseAsync<TResponse>(this HttpClient httpClient, string url)
    {
        try
        {
            var httpResponse = await httpClient.GetAsync(url);

            var resultContent = await HandleResponseAsync(httpResponse, url);

            // Deserialize the response content into TResponse
            var response = await Task.Run(() => JsonSerializer.Deserialize<TResponse>(resultContent));
            if (response == null)
            {
                throw new InvalidOperationException("Response content could not be deserialized.");
            }

            return response;
        }
        catch (HttpRequestException httpEx)
        {
            throw new HttpRequestException($"An error occurred while making the HTTP request to {url}.", httpEx);
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"An unexpected error occurred while making the HTTP request to {url}.", ex);
        }
    }
}
