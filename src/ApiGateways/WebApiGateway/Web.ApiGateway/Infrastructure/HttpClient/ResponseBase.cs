namespace Web.ApiGateway.Infrastructure.HttpClient;

public class ResponseBase
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}