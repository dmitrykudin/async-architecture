using System.Linq;
using System.Threading.Tasks;
using AsyncArchitecture.TaskTracker.Interfaces.Commands;
using AsyncArchitecture.TaskTracker.Interfaces.Queries;
using AsyncArchitecture.TaskTracker.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AsyncArchitecture.TaskTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoItemController : Controller
    {
        private IUserQueries UserQueries { get; }

        private IToDoItemQueries ToDoItemQueries { get; }

        private IToDoItemCommands ToDoItemCommands { get; }

        private IMapper Mapper { get; }

        private readonly ILogger<HomeController> _logger;

        public ToDoItemController(
            IUserQueries userQueries,
            IToDoItemQueries toDoItemQueries,
            IToDoItemCommands toDoItemCommands,
            IMapper mapper,
            ILogger<HomeController> logger)
        {
            UserQueries = userQueries;
            ToDoItemQueries = toDoItemQueries;
            ToDoItemCommands = toDoItemCommands;
            Mapper = mapper;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var toDoItems = await ToDoItemQueries.GetAllToDoItemsAsync();
            var toDoItemsViewModels = toDoItems.Select(x => Mapper.Map<ToDoItemViewModel>(x));

            return View(toDoItemsViewModels);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateToDoItem([FromForm] string title, [FromForm] string description)
        {
            await ToDoItemCommands.CreateToDoItemAsync(title, description);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("assign")]
        public async Task<IActionResult> AssignToDoItem([FromForm] int id)
        {
            await AssignToDoItemAsync(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("shuffle")]
        public async Task<IActionResult> ShuffleToDoItems()
        {
            var toDoItems = await ToDoItemQueries.GetUnfinishedToDoItemsAsync();
            foreach (var toDoItem in toDoItems)
            {
                await AssignToDoItemAsync(toDoItem.Id);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("finish")]
        public async Task<IActionResult> FinishToDoItem([FromForm] int id)
        {
            await ToDoItemCommands.FinishToDoItemAsync(id);

            return RedirectToAction("Index");
        }

        private async Task AssignToDoItemAsync(int id)
        {
            var user = await UserQueries.GetRandomUser();
            await ToDoItemCommands.AssignToDoItemAsync(id, user.Id);
        }
    }
}