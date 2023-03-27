using AutoMapper;
using TasksHandler.Entities;
using TasksHandler.Models;

namespace TasksHandler.Services
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() {
            CreateMap<Tasks, TaskDTO>()
                .ForMember(dto => dto.TotalSteps, ent => ent.MapFrom(x => x.Steps.Count()))
                .ForMember(dto => dto.DoneSteps, ent => 
                ent.MapFrom(x => x.Steps.Where(p => p.Done).Count()));
        }
    }
}
