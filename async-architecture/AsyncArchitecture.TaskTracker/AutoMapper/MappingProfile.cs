using AsyncArchitecture.TaskTracker.Database.Entities;
using AsyncArchitecture.TaskTracker.ViewModels;
using AutoMapper;

namespace AsyncArchitecture.TaskTracker.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ToDoItem, ToDoItemViewModel>();
            CreateMap<User, UserViewModel>();
        }
    }
}