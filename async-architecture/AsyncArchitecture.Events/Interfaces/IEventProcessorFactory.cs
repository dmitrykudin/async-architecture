namespace AsyncArchitecture.Events.Interfaces
{
    public interface IEventProcessorFactory
    {
        IEventProcessor GetEventProcessorByTopicName(string topicName);
    }
}