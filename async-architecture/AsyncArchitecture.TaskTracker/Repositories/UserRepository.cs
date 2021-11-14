using System;
using System.Threading.Tasks;
using AsyncArchitecture.TaskTracker.Database;
using AsyncArchitecture.TaskTracker.Database.Entities;
using AsyncArchitecture.TaskTracker.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AsyncArchitecture.TaskTracker.Repositories
{
    public class UserRepository : IUserRepository
    {
        private TaskTrackerDbContext Context { get; }

        private ILogger<UserRepository> Logger { get; }

        public UserRepository(
            TaskTrackerDbContext context,
            ILogger<UserRepository> logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task<User> GetUserAsync(int id)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                Logger.LogError("User not found. Id: {id}", id);
                throw new ArgumentException("User not found. Id: " + id);
            }

            return user;
        }

        public async Task<User> GetUserAsync(Guid publicId)
        {
            return await Context.Users.FirstOrDefaultAsync(x => x.PublicId == publicId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var userFromDb = await GetUserAsync(user.PublicId);

            if (userFromDb == null)
            {
                await Context.Users.AddAsync(user);
                await Context.SaveChangesAsync();

                return user;
            }

            return await UpdateUserAsync(user);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var userFromDb = await GetUserAsync(user.PublicId);

            Context.Entry(userFromDb).CurrentValues.SetValues(user);
            await Context.SaveChangesAsync();

            return userFromDb;
        }
    }
}