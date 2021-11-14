using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncArchitecture.TaskTracker.Database;
using AsyncArchitecture.TaskTracker.Database.Entities;
using AsyncArchitecture.TaskTracker.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AsyncArchitecture.TaskTracker.Queries
{
    public class ToDoItemQueries : IToDoItemQueries
    {
        private TaskTrackerDbContext Context { get; }

        private ILogger<ToDoItemQueries> Logger { get; }

        public ToDoItemQueries(TaskTrackerDbContext context, ILogger<ToDoItemQueries> logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task<IEnumerable<ToDoItem>> GetAllToDoItemsAsync()
        {
            return await Context.ToDoItems
                .Include(x => x.Assignee)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<ToDoItem>> GetUnfinishedToDoItemsAsync()
        {
            return await Context.ToDoItems
                .Where(x => !x.IsFinished)
                .Include(x => x.Assignee)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<ToDoItem>> GetUserToDoItemsAsync(int userId)
        {
            return await Context.ToDoItems
                .Include(x => x.Assignee)
                .Where(x => x.AssigneeId == userId)
                .ToArrayAsync();
        }
    }
}