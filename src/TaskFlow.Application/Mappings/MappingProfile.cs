using AutoMapper;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<TaskItem, TaskResponse>()
            .ForMember(dest => dest.IsOverdue, 
                opt => opt.MapFrom(src => src.DueDate < DateTime.UtcNow && src.Status != ETaskStatus.Completed))
            .ForMember(dest => dest.UserName, 
                opt => opt.MapFrom(src => src.User.Name));
        CreateMap<TaskRequest, TaskItem>()
            .ForMember(dest => dest.Status, opt => opt.Ignore());
    }
}
