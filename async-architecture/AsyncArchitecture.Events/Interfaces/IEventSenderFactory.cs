namespace AsyncArchitecture.Events.Interfaces
{
    public interface IEventSenderFactory
    {
        IEventSender Create(string server, string topicName);
    }
}