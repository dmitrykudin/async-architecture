using AsyncArchitecture.TaskTracker.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace AsyncArchitecture.TaskTracker.Database
{
    public class TaskTrackerDbContext : DbContext
    {
        public DbSet<ToDoItem> ToDoItems { get; set; }

        public DbSet<User> Users { get; set; }

        public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ToDoItem>(entity => entity.ToTable("ToDoItems"));
            builder.Entity<User>(entity => entity.ToTable("Users"));
        }
    }
}