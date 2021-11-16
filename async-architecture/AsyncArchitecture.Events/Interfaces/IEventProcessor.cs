using System.Threading.Tasks;

namespace AsyncArchitecture.Events.Interfaces
{
    public interface IEventProcessor
    {
        Task ProcessAsync(string @event);
    }
}