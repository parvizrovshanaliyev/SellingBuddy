namespace Web.ApiGateway.Infrastructure.HttpClient;

public class HttpClientDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HttpClientDelegatingHandler> _logger;

    public HttpClientDelegatingHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<HttpClientDelegatingHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Extract the Authorization header from the HttpContext
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                // Remove any existing Authorization header and add the one from HttpContext
                if (request.Headers.Contains("Authorization"))
                {
                    request.Headers.Remove("Authorization");
                    _logger.LogDebug("Existing Authorization header removed from the HTTP request.");
                }

                request.Headers.Add("Authorization", authHeader);
                _logger.LogDebug("Authorization header added to the HTTP request.");
            }
            else
            {
                _logger.LogWarning("No Authorization token found in HttpContext. Proceeding without authentication.");
            }

            // Log request details
            _logger.LogInformation("Sending {Method} request to {Uri}", request.Method, request.RequestUri);

            // Send the request
            var response = await base.SendAsync(request, cancellationToken);

            // Log based on response status
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Request to {Uri} succeeded with status code {StatusCode}.", request.RequestUri,
                    response.StatusCode);
            }
            else
            {
                _logger.LogWarning("Request to {Uri} failed with status code {StatusCode}. Reason: {ReasonPhrase}.",
                    request.RequestUri, response.StatusCode, response.ReasonPhrase);
            }

            return response;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Request to {Uri} timed out.", request.RequestUri);
            throw new TimeoutException($"Request to {request.RequestUri} timed out.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "A network error occurred while sending the request to {Uri}.", request.RequestUri);
            throw new HttpRequestException($"Network error while sending the request to {request.RequestUri}.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An unexpected error occurred while sending the request to {Uri}.",
                request.RequestUri);
            throw;
        }
    }
}