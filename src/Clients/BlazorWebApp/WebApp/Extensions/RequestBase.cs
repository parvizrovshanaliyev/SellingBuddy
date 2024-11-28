using System;

namespace WebApp.Domain.User;

public class RequestBase
{
    // Common properties or methods for all requests can go here
    public DateTime RequestTime { get; set; } = DateTime.Now;
}