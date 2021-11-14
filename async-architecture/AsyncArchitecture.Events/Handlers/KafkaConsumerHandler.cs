using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncArchitecture.Events.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AsyncArchitecture.Events.Handlers
{
    public class KafkaConsumerHandler : IHostedService
    {
        private IServiceProvider ServiceProvider { get; }

        private string Server { get; }

        private string TopicName { get; }

        public KafkaConsumerHandler(IServiceProvider serviceProvider, string server, string topicName)
        {
            ServiceProvider = serviceProvider;
            Server = server;
            TopicName = topicName;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = "st_consumer_group",
                BootstrapServers = Server,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using (var builder = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                builder.Subscribe(TopicName);
                var cancelToken = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        var consumeResult = builder.Consume(cancelToken.Token);

                        using (var scope = ServiceProvider.CreateScope())
                        {
                            var eventProcessorFactory =
                                scope.ServiceProvider.GetRequiredService<IEventProcessorFactory>();
                            var eventProcessor = eventProcessorFactory.GetEventProcessorByTopicName(TopicName);

                            await eventProcessor.ProcessAsync(consumeResult.Message.Value);
                        }

                        Console.WriteLine($"Message: {consumeResult.Message.Value} received from {consumeResult.TopicPartitionOffset}");
                    }
                }
                catch (Exception)
                {
                    builder.Close();
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}