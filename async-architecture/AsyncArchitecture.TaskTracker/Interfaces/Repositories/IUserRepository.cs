using System;
using System.Threading.Tasks;
using AsyncArchitecture.TaskTracker.Database.Entities;

namespace AsyncArchitecture.TaskTracker.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(int id);

        Task<User> GetUserAsync(Guid publicId);

        Task<User> CreateUserAsync(User user);

        Task<User> UpdateUserAsync(User user);
    }
}