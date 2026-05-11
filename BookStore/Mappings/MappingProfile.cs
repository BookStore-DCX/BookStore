using AutoMapper;
using BookStore.DTOs.BookCondition;
using BookStore.DTOs.Inventory;
using BookStore.DTOs.Review;
using BookStore.Models;

namespace BookStore.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Bookreview, ReviewDto>().ReverseMap();
            CreateMap<ReviewCreateDto, Bookreview>();
           
            CreateMap<Bookcondition, BookConditionDto>();

            CreateMap<Inventory, InventoryDto>()
                .ForMember(d => d.ConditionDescription, o => o.MapFrom(s => s.RanksNavigation != null ? s.RanksNavigation.Description : null))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.RanksNavigation != null ? s.RanksNavigation.Price : null));
            CreateMap<InventoryCreateDto, Inventory>();

            
        }
    }

}
