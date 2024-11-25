using System.Security.Authentication;
using System.Threading.Tasks;

namespace WebApp.Application.Services.Identity;

public interface IIdentityService
{
    string GetUserName();
    
    string GetUserToken();

    bool IsAuthenticated { get; }

    Task<bool> Login(string username, string password);
    
    void Logout();
}