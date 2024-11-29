using System;
using Microsoft.AspNetCore.Components;

namespace WebApp.Infrastructure;

public class AppStateManager
{
    public event Action<ComponentBase ,string> StateChanged;
    public void UpdateCartCount(ComponentBase component)
  
    {
        StateChanged?.Invoke(component, "updatebasket");
    }
    
    public void LoginChanged(ComponentBase component)
    {
        StateChanged?.Invoke(component, "login");
    }
}