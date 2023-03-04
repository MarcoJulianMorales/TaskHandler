using AutoMapper;
using TasksHandler.Entities;
using TasksHandler.Models;

namespace TasksHandler.Services
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() {
            CreateMap<Tasks, TaskDTO>();
        }
    }
}
