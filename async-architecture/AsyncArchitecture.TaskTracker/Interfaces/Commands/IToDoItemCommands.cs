using System.Threading.Tasks;
using AsyncArchitecture.TaskTracker.Database.Entities;

namespace AsyncArchitecture.TaskTracker.Interfaces.Commands
{
    public interface IToDoItemCommands
    {
        Task<ToDoItem> CreateToDoItemAsync(string title, string description);

        Task<ToDoItem> AssignToDoItemAsync(int toDoItemId, int userId);

        Task<ToDoItem> FinishToDoItemAsync(int toDoItemId);
    }
}