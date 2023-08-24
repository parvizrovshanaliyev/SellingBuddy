using System;
using System.Text.Json.Serialization;


//using Newtonsoft.Json;

namespace EventBus.Base.Events
{
    public class IntegrationEvent
    {

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createdDate)
        {
            Id = id;
            CreatedDate = createdDate;
        }

        public  Guid Id { get; private set; }

        public DateTime CreatedDate { get; private set; }

    }
}
