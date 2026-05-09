using BookStore.DTOs.Auth;
using BookStore.DTOs.User;
using BookStore.Models;
using AutoMapper;

namespace BookStore.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, User>();
            CreateMap<User, UserResponseDto>()
                .ForMember(d => d.RoleName, o => o.MapFrom(s => s.RoleNumberNavigation != null ? s.RoleNumberNavigation.PermRole1 : null));
            CreateMap<UserUpdateDto, User>();

        }
    }
}