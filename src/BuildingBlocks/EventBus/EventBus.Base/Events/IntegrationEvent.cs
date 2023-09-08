using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventBus.Base.Events;

public abstract class IntegrationEvent
{
    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.Now;
    }

    [JsonConstructor]
    protected IntegrationEvent(Guid id, DateTime createdDate)
    {
        Id = id;
        CreatedDate = createdDate;
    }

    public Guid Id { get; }

    public DateTime CreatedDate { get; }
    
    
    public static string Serialize(IntegrationEvent integrationEvent)
    {
        return JsonSerializer.Serialize(integrationEvent, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        });
    }
    
}