namespace AsyncArchitecture.TaskTracker.ViewModels
{
    public class ToDoItemViewModel
    {
        public int Id { get; set; }

        public string PublicId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsFinished { get; set; }

        public UserViewModel Assignee { get; set; }
    }
}