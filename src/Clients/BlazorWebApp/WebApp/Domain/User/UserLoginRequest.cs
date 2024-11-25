namespace WebApp.Domain.User;

public class UserLoginRequest
{
    public UserLoginRequest(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; set; }
    public string Password { get; set; }
}