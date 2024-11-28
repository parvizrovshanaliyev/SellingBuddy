using System;

namespace WebApp.Domain.User;

/// <summary>
/// Represents a request to log in a user with a username and password.
/// </summary>
public class UserLoginRequest : RequestBase
{
    public UserLoginRequest()
    {
        
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="UserLoginRequest"/> class.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <exception cref="ArgumentException">Thrown when either the username or password is null or empty.</exception>
    public UserLoginRequest(string username, string password) : this()
    {

        Username = username;
        Password = password;
    }

    /// <summary>
    /// Gets the username of the user.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets the password of the user.
    /// </summary>
    public string Password { get; set; }
}

