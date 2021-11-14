using System;
using AsyncArchitecture.Events.Interfaces;
using AsyncArchitecture.Events.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncArchitecture.Events.Services
{
    public class EventProcessorFactory : IEventProcessorFactory
    {
        private readonly IServiceProvider ServiceProvider;

        private EventProcessorConfiguration Configuration { get; }

        public EventProcessorFactory(
            IServiceProvider serviceProvider,
            EventProcessorConfiguration configuration)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
        }

        public IEventProcessor GetEventProcessorByTopicName(string topicName)
        {
            return Configuration.TopicToEventProcessorMapping.TryGetValue(topicName, out var processorType)
                ? ServiceProvider.GetRequiredService(processorType) as IEventProcessor
                : null;
        }
    }
}