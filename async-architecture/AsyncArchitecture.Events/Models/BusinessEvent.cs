using System;

namespace AsyncArchitecture.Events.Models
{
    public class BusinessEvent<TEntity, TEvent> : Event
    {
        public Type EntityType => typeof(TEntity);

        public TEvent Event { get; set; }

        public dynamic Data { get; set; }
    }
}