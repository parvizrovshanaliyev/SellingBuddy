using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace WebApp.Infrastructure.HttpClient;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly ISyncLocalStorageService _storageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthTokenHandler"/> class.
    /// </summary>
    /// <param name="storageService">The service to access local storage for the auth token.</param>
    public AuthTokenHandler(ISyncLocalStorageService storageService)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    }

    /// <summary>
    /// Adds the authorization token to the request header if it exists in the local storage.
    /// </summary>
    /// <param name="request">The HTTP request message to which the token will be added.</param>
    /// <param name="cancellationToken">The cancellation token to observe while sending the request.</param>
    /// <returns>The HTTP response message from the subsequent handler.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Check if the token exists in the local storage and add it to the request header
        if (_storageService.ContainKey("token"))
        {
            var token = _storageService.GetItem<string>("token");

            // Ensure the token is not null or empty before adding to the header
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Pass the request to the next handler in the pipeline
        return await base.SendAsync(request, cancellationToken);
    }
}