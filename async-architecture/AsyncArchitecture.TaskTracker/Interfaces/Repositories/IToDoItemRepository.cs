using System;
using System.Threading.Tasks;
using AsyncArchitecture.TaskTracker.Database.Entities;

namespace AsyncArchitecture.TaskTracker.Interfaces.Repositories
{
    public interface IToDoItemRepository
    {
        Task<ToDoItem> GetToDoItemAsync(int id);

        Task<ToDoItem> CreateToDoItemAsync(string title, string description);

        Task<ToDoItem> UpdateToDoItemAsync(int id, Action<ToDoItem> update);
    }
}