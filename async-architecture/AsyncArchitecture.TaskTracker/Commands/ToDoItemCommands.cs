using System.Threading.Tasks;
using AsyncArchitecture.Events.Interfaces;
using AsyncArchitecture.Events.Models;
using AsyncArchitecture.TaskTracker.Database.Entities;
using AsyncArchitecture.TaskTracker.Interfaces.Commands;
using AsyncArchitecture.TaskTracker.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace AsyncArchitecture.TaskTracker.Commands
{
    public class ToDoItemCommands : IToDoItemCommands
    {
        private IToDoItemRepository ToDoItemRepository { get; }

        private IUserRepository UserRepository { get; }

        private IEventSender TaskBusinessEventsSender { get; }

        private IEventSender TaskEventsStreamSender { get; }

        private ILogger<ToDoItemCommands> Logger { get; }

        private const string TaskEventsServer = "localhost:9092";

        private const string TaskEventsTopic = "task_events";

        private const string TasksStreamingTopic = "task_stream";

        public ToDoItemCommands(
            IToDoItemRepository toDoItemRepository,
            IUserRepository userRepository,
            IEventSenderFactory eventSenderFactory,
            ILogger<ToDoItemCommands> logger)
        {
            ToDoItemRepository = toDoItemRepository;
            UserRepository = userRepository;
            TaskBusinessEventsSender = eventSenderFactory.Create(TaskEventsServer, TaskEventsTopic);
            TaskEventsStreamSender = eventSenderFactory.Create(TaskEventsServer, TasksStreamingTopic);
            Logger = logger;
        }

        public async Task<ToDoItem> CreateToDoItemAsync(string title, string description)
        {
            var toDoItem = await ToDoItemRepository.CreateToDoItemAsync(title, description);

            await TaskBusinessEventsSender.SendEvent(new BusinessEvent<ToDoItem, ToDoItemBusinessEventType>
            {
                Data = new { PublicId = toDoItem.PublicId },
                Event = ToDoItemBusinessEventType.Created
            });

            await TaskEventsStreamSender.SendEvent(new CudEvent<ToDoItem>
            {
                Entity = toDoItem,
                CudEventType = CudEventType.Create
            });

            return toDoItem;
        }

        public async Task<ToDoItem> AssignToDoItemAsync(int toDoItemId, int userId)
        {
            var user = await UserRepository.GetUserAsync(userId);
            var toDoItem = await ToDoItemRepository.UpdateToDoItemAsync(toDoItemId, toDoItem =>
            {
                toDoItem.AssigneeId = user.Id;
            });

            await TaskBusinessEventsSender.SendEvent(new BusinessEvent<ToDoItem, ToDoItemBusinessEventType>
            {
                Data = new
                {
                    UserId = user.PublicId,
                    PublicId = toDoItem.PublicId,
                },
                Event = ToDoItemBusinessEventType.Assigned,
            });

            await TaskEventsStreamSender.SendEvent(new CudEvent<ToDoItem>
            {
                Entity = toDoItem,
                CudEventType = CudEventType.Update
            });

            return toDoItem;
        }

        public async Task<ToDoItem> FinishToDoItemAsync(int toDoItemId)
        {
            var toDoItem = await ToDoItemRepository.UpdateToDoItemAsync(toDoItemId, toDoItem =>
            {
                toDoItem.IsFinished = true;
            });

            await TaskBusinessEventsSender.SendEvent(new BusinessEvent<ToDoItem, ToDoItemBusinessEventType>
            {
                Data = new { PublicId = toDoItem.PublicId },
                Event = ToDoItemBusinessEventType.Finished,
            });

            await TaskEventsStreamSender.SendEvent(new CudEvent<ToDoItem>
            {
                Entity = toDoItem,
                CudEventType = CudEventType.Update
            });

            return toDoItem;
        }
    }
}