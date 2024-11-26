using System.Threading.Tasks;

namespace WebApp.Application.Services.Identity;

/// <summary>
/// Provides methods for managing user authentication and identity.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Retrieves the username of the currently authenticated user.
    /// </summary>
    /// <returns>The username of the current user.</returns>
    string GetUserName();

    /// <summary>
    /// Retrieves the authentication token of the currently authenticated user.
    /// </summary>
    /// <returns>The authentication token.</returns>
    string GetUserToken();

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    /// <value>
    /// <c>true</c> if the user is authenticated (i.e., a valid token exists); otherwise, <c>false</c>.
    /// </value>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Logs in the user using the specified username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if login was successful; otherwise, <c>false</c>.</returns>
    Task<bool> Login(string username, string password);

    /// <summary>
    /// Logs out the current user by clearing the stored token and username.
    /// </summary>
    void Logout();
}
