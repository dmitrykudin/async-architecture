using System;
using System.Threading.Tasks;
using AsyncArchitecture.TaskTracker.Database;
using AsyncArchitecture.TaskTracker.Database.Entities;
using AsyncArchitecture.TaskTracker.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AsyncArchitecture.TaskTracker.Repositories
{
    public class ToDoItemRepository : IToDoItemRepository
    {
        private TaskTrackerDbContext Context { get; }

        private ILogger<ToDoItemRepository> Logger { get; }

        public ToDoItemRepository(
            TaskTrackerDbContext context,
            ILogger<ToDoItemRepository> logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task<ToDoItem> GetToDoItemAsync(int id)
        {
            var toDoItem = await Context.ToDoItems.FirstOrDefaultAsync(x => x.Id == id);

            if (toDoItem == null)
            {
                Logger.LogError("ToDoItem not found. Id: {id}", id);
                throw new ArgumentException("ToDoItem not found. Id: " + id);
            }

            return toDoItem;
        }

        public async Task<ToDoItem> CreateToDoItemAsync(string title, string description)
        {
            var toDoItem = new ToDoItem
            {
                PublicId = Guid.NewGuid(),
                Title = title,
                Description = description
            };

            await Context.ToDoItems.AddAsync(toDoItem);
            await Context.SaveChangesAsync();

            return toDoItem;
        }

        public async Task<ToDoItem> UpdateToDoItemAsync(int id, Action<ToDoItem> update)
        {
            var toDoItem = await GetToDoItemAsync(id);

            update(toDoItem);

            await Context.SaveChangesAsync();
            return toDoItem;
        }
    }
}