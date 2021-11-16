using System;
using System.Threading.Tasks;
using AsyncArchitecture.Events.Interfaces;
using AsyncArchitecture.Events.Models;
using AsyncArchitecture.TaskTracker.Database.Entities;
using AsyncArchitecture.TaskTracker.Interfaces.Repositories;
using Newtonsoft.Json;

namespace AsyncArchitecture.TaskTracker.Processors
{
    public class UserStreamProcessor : IEventProcessor
    {
        private IUserRepository UserRepository { get; }

        public UserStreamProcessor(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public async Task ProcessAsync(string @event)
        {
            var userCudEvent = JsonConvert.DeserializeObject<CudEvent<User>>(@event);

            switch (userCudEvent.CudEventType)
            {
                case CudEventType.Create:
                    await UserRepository.CreateUserAsync(userCudEvent.Entity);
                    break;
                case CudEventType.Update:
                    await UserRepository.UpdateUserAsync(userCudEvent.Entity);
                    break;
                case CudEventType.Delete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}