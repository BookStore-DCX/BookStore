using AutoMapper;
using BookStore.DTOs.PurchaseLog;
using BookStore.DTOs.ShoppingCart;
using BookStore.DTOs.User;
using BookStore.Models;

namespace BookStore.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.RoleName, o => o.MapFrom(s => s.RoleNumberNavigation != null ? s.RoleNumberNavigation.PermRole1 : null));
            CreateMap<UserUpdateDto, User>();

            CreateMap<Shoppingcart, ShoppingCartDto>()
                .ForMember(d => d.BookTitle, o => o.MapFrom(s => s.IsbnNavigation != null ? s.IsbnNavigation.Title : null));
            CreateMap<ShoppingCartCreateDto, Shoppingcart>();

            CreateMap<Purchaselog, PurchaseLogDto>().ReverseMap();
            CreateMap<PurchaseLogCreateDto, Purchaselog>();
        }
    }
}
