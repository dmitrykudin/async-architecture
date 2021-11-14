using System;
using System.Collections.Generic;

namespace AsyncArchitecture.Events.Models
{
    public class EventProcessorConfiguration
    {
        public Dictionary<string, Type> TopicToEventProcessorMapping { get; set; }
    }
}