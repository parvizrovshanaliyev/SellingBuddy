namespace WebApp.Domain.User;

public class ResponseBase
{
    // Common properties or methods for all responses can go here
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}