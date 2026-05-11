using AutoMapper;
using BookStore.DTOs.Category;
using BookStore.DTOs.Publisher;
using BookStore.DTOs.State;
using BookStore.Models;

namespace BookStore.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
       
            CreateMap<Publisher, PublisherDto>()
                .ForMember(
                    d => d.StateName,
                    o => o.MapFrom(
                        s => s.StateCodeNavigation != null
                            ? s.StateCodeNavigation.StateName
                            : null
                    ));

            CreateMap<PublisherCreateDto, Publisher>();

            CreateMap<Category, CategoryDto>();

  
            CreateMap<State, StateDto>();
        }
    }
}