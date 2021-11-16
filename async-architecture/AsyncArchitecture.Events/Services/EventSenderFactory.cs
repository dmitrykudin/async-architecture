using AsyncArchitecture.Events.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace AsyncArchitecture.Events.Services
{
    public class EventSenderFactory : IEventSenderFactory
    {
        private ILoggerFactory LoggerFactory { get; }

        public EventSenderFactory(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }

        public IEventSender Create(string server, string topicName)
        {
            return new KafkaEventSender(
                new ProducerConfig { BootstrapServers = server },
                topicName,
                LoggerFactory.CreateLogger<KafkaEventSender>());
        }
    }
}