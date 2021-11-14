using System;
using System.Threading.Tasks;
using AsyncArchitecture.Events.Interfaces;
using AsyncArchitecture.Events.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AsyncArchitecture.Events.Services
{
    public class KafkaEventSender : IEventSender
    {
        private ILogger<KafkaEventSender> Logger { get; }

        private ProducerConfig Config { get; }

        private string TopicName { get; }

        public KafkaEventSender(
            ProducerConfig config,
            string topicNameName,
            ILogger<KafkaEventSender> logger)
        {
            Config = config;
            TopicName = topicNameName;
            Logger = logger;
        }

        public async Task SendEvent(Event @event)
        {
            var message = JsonConvert.SerializeObject(@event);
            await SendEvent((string)message);
        }

        public async Task SendEvent(string message)
        {
            using (var producer = new ProducerBuilder<Null, string>(Config).Build())
            {
                try
                {
                    await producer.ProduceAsync(TopicName, new Message<Null, string> { Value = message });
                }
                catch (Exception e)
                {
                    Logger.LogError("Error during event sending: " + e.Message);
                }
            }
        }
    }
}