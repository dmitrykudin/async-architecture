using System;
using System.Linq;
using System.Threading.Tasks;
using AsyncArchitecture.TaskTracker.Database;
using AsyncArchitecture.TaskTracker.Database.Entities;
using AsyncArchitecture.TaskTracker.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AsyncArchitecture.TaskTracker.Queries
{
    public class UserQueries : IUserQueries
    {
        private TaskTrackerDbContext Context { get; }

        private ILogger<ToDoItemQueries> Logger { get; }

        public UserQueries(TaskTrackerDbContext context, ILogger<ToDoItemQueries> logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await Context.Users.FirstOrDefaultAsync(x => x.PublicId == userId)
                   ?? throw new ArgumentException("User not found");
        }

        public async Task<User> GetRandomUser()
        {
            var count = await Context.Users.CountAsync();
            var rand = new Random();
            return await Context.Users.Skip(rand.Next(count)).FirstOrDefaultAsync();
        }
    }
}