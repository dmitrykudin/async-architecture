using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AsyncArchitecture.TaskTracker.Database.Entities
{
    public class ToDoItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid PublicId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsFinished { get; set; }

        public int? AssigneeId { get; set; }

        [JsonIgnore]
        public User Assignee { get; set; }
    }
}