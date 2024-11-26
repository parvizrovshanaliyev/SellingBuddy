using System;

namespace WebApp.Domain.User;

/// <summary>
/// Represents the response returned after a successful login attempt.
/// </summary>
public class UserLoginResponse : ResponseBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserLoginResponse"/> class.
    /// </summary>
    /// <param name="username">The username of the authenticated user.</param>
    /// <param name="token">The authentication token returned after a successful login.</param>
    /// <exception cref="ArgumentException">Thrown if either username or token is null or empty.</exception>
    public UserLoginResponse(string username, string token) : this()
    {
        // Validate that username and token are not null or empty
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty or null.", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be empty or null.", nameof(token));
        }

        Username = username;
        Token = token;
    }

    public UserLoginResponse()
    {
       
    }

    /// <summary>
    /// Gets the username of the authenticated user.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the authentication token for the user.
    /// </summary>
    public string Token { get; }
}
