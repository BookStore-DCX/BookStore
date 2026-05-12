using AutoMapper;
using BookStore.DTOs.Auth;
using BookStore.DTOs.Category;
using BookStore.DTOs.Publisher;
using BookStore.DTOs.State;
using BookStore.DTOs.User;
using BookStore.Models;

namespace BookStore.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- Publisher Mappings ---
            CreateMap<Publisher, PublisherDto>()
                .ForMember(
                    d => d.StateName,
                    o => o.MapFrom(
                        s => s.StateCodeNavigation != null
                            ? s.StateCodeNavigation.StateName
                            : null
                    ));

            CreateMap<PublisherCreateDto, Publisher>();

            // --- Category & State Mappings ---
            CreateMap<Category, CategoryDto>();
            CreateMap<State, StateDto>();

            // --- User & Auth Mappings ---
            CreateMap<RegisterDto, User>();
            CreateMap<UserUpdateDto, User>();
        }
    }
}