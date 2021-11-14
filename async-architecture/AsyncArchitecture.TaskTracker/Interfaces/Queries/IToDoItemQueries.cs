using System.Collections.Generic;
using System.Threading.Tasks;
using AsyncArchitecture.TaskTracker.Database.Entities;

namespace AsyncArchitecture.TaskTracker.Interfaces.Queries
{
    public interface IToDoItemQueries
    {
        Task<IEnumerable<ToDoItem>> GetAllToDoItemsAsync();

        Task<IEnumerable<ToDoItem>> GetUnfinishedToDoItemsAsync();

        Task<IEnumerable<ToDoItem>> GetUserToDoItemsAsync(int userId);
    }
}