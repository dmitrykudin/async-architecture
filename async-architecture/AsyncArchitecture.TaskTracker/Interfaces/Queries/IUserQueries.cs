using System;
using System.Threading.Tasks;
using AsyncArchitecture.TaskTracker.Database.Entities;

namespace AsyncArchitecture.TaskTracker.Interfaces.Queries
{
    public interface IUserQueries
    {
        Task<User> GetUser(Guid userId);

        Task<User> GetRandomUser();
    }
}