using System.Threading.Tasks;
using AsyncArchitecture.Events.Models;

namespace AsyncArchitecture.Events.Interfaces
{
    public interface IEventSender
    {
        Task SendEvent(Event @event);

        Task SendEvent(string message);
    }
}