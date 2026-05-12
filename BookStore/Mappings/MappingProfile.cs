using AutoMapper;
using BookStore.DTOs.Auth;
using BookStore.DTOs.Author;
using BookStore.DTOs.Book;
using BookStore.DTOs.BookCondition;
using BookStore.DTOs.Category;
using BookStore.DTOs.Inventory;
using BookStore.DTOs.Publisher;
using BookStore.DTOs.PurchaseLog;
using BookStore.DTOs.Review;
using BookStore.DTOs.ShoppingCart;
using BookStore.DTOs.State;
using BookStore.DTOs.User;
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

            CreateMap<RegisterDto, User>();

            CreateMap<UserUpdateDto, User>();

            CreateMap<Shoppingcart, ShoppingCartDto>()
                .ForMember(
                    d => d.BookTitle,
                    o => o.MapFrom(
                        s => s.IsbnNavigation != null
                            ? s.IsbnNavigation.Title
                            : null
                    ));

            CreateMap<ShoppingCartCreateDto, Shoppingcart>();

            CreateMap<Purchaselog, PurchaseLogDto>().ReverseMap();

            CreateMap<PurchaseLogCreateDto, Purchaselog>();

            CreateMap<Bookreview, ReviewDto>().ReverseMap();

            CreateMap<Bookcondition, BookConditionDto>();

            CreateMap<Inventory, InventoryDto>()
                .ForMember(
                    d => d.ConditionDescription,
                    o => o.MapFrom(s => s.RanksNavigation != null ? s.RanksNavigation.Description : null))
                .ForMember(
                    d => d.Price,
                    o => o.MapFrom(s => s.RanksNavigation != null ? s.RanksNavigation.Price : null));

            CreateMap<InventoryCreateDto, Inventory>();

            CreateMap<Book, BookDto>()
                .ForMember(
                    d => d.CategoryName,
                    o => o.MapFrom(s => s.CategoryNavigation != null ? s.CategoryNavigation.CatDescription : null))
                .ForMember(
                    d => d.PublisherName,
                    o => o.MapFrom(s => s.Publisher != null ? s.Publisher.Name : null));

            CreateMap<BookCreateDto, Book>();

            CreateMap<BookUpdateDto, Book>();

            CreateMap<Author, AuthorDto>();

            CreateMap<AuthorCreateDto, Author>();

            CreateMap<Bookauthor, BookAuthorDto>().ReverseMap();
        }
    }
}