namespace WebApp.Infrastructure.HttpClient;

public class ResponseBase
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}