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
        Username = username;
        UserToken = token;
    }

    public UserLoginResponse()
    {

    }

    public string Username { get; set; }
    public string UserToken { get; set; }
}
