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
                    ))
                .ForMember(
                    d => d.InventoryId,
                    o => o.MapFrom(s => GetDisplayCopy(s) != null ? GetDisplayCopy(s)!.InventoryId : (int?)null))
                .ForMember(
                    d => d.Condition,
                    o => o.MapFrom(s => GetDisplayCopy(s) != null ? GetDisplayCopy(s)!.RanksNavigation.Description : null))
                .ForMember(
                    d => d.Price,
                    o => o.MapFrom(s => GetDisplayCopy(s) != null ? GetDisplayCopy(s)!.RanksNavigation.Price : null))
                .ForMember(
                    d => d.AvailableCopies,
                    o => o.MapFrom(s => GetAvailableCopies(s)));

            CreateMap<Inventory, ShoppingCartCopyDto>()
                .ForMember(
                    d => d.Condition,
                    o => o.MapFrom(s => s.RanksNavigation.Description))
                .ForMember(
                    d => d.Price,
                    o => o.MapFrom(s => s.RanksNavigation.Price));

            CreateMap<ShoppingCartCreateDto, Shoppingcart>();

            CreateMap<Purchaselog, DTOs.PurchaseLog.PurchaseLogDto>()
                .ForMember(
                    d => d.BookTitle,
                    o => o.MapFrom(s => s.Inventory != null ? s.Inventory.IsbnNavigation.Title : null))
                .ForMember(
                    d => d.AuthorName,
                    o => o.MapFrom(s => s.Inventory != null
                        ? string.Join(", ", s.Inventory.IsbnNavigation.Bookauthors
                            .OrderByDescending(a => a.PrimaryAuthor == "Y")
                            .ThenBy(a => a.Author.LastName)
                            .ThenBy(a => a.Author.FirstName)
                            .Select(a => $"{a.Author.FirstName} {a.Author.LastName}".Trim()))
                        : null))
                .ForMember(
                    d => d.Condition,
                    o => o.MapFrom(s => s.Inventory != null ? s.Inventory.RanksNavigation.Description : null))
                .ForMember(
                    d => d.Price,
                    o => o.MapFrom(s => s.Inventory != null ? s.Inventory.RanksNavigation.Price : null));

            CreateMap<DTOs.PurchaseLog.PurchaseLogCreateDto, Purchaselog>();

            CreateMap<Bookreview, ReviewDto>()
                .ForMember(
                    d => d.BookName,
                    o => o.MapFrom(s => s.IsbnNavigation != null ? s.IsbnNavigation.Title : null))
                .ReverseMap();

            CreateMap<Bookcondition, BookConditionDto>();

            CreateMap<Inventory, InventoryDto>()
                .ForMember(
                    d => d.BookTitle,
                    o => o.MapFrom(s => s.IsbnNavigation != null ? s.IsbnNavigation.Title : null))
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
                    o => o.MapFrom(s => s.Publisher != null ? s.Publisher.Name : null))
                .ForMember(
                    d => d.InventoryCount,
                    o => o.MapFrom(s => s.Inventories.Count(i => i.Purchased == 0)));

            CreateMap<BookCreateDto, Book>();

            CreateMap<BookUpdateDto, Book>();

            CreateMap<Author, AuthorDto>();

            CreateMap<AuthorCreateDto, Author>();

            CreateMap<Bookauthor, BookAuthorDto>().ReverseMap();

            CreateMap<Inventory, BookCopyDto>()
                .ForMember(d => d.Condition, o => o.MapFrom(s => s.RanksNavigation!.Description))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.RanksNavigation!.Price));

            CreateMap<Book, BookDetailDto>()
                .ForMember(d => d.Edition, o => o.MapFrom(s => s.Edition))
                .ForMember(d => d.CategoryId, o => o.MapFrom(s => s.Category))
                .ForMember(d => d.PublisherId, o => o.MapFrom(s => s.PublisherId))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.CategoryNavigation!.CatDescription))
                .ForMember(d => d.Publisher, o => o.MapFrom(s => s.Publisher!.Name))
                .ForMember(d => d.Authors, o => o.MapFrom(s => s.Bookauthors.Select(a => $"{a.Author.FirstName} {a.Author.LastName}".Trim())))
                .ForMember(d => d.Copies, o => o.MapFrom(s => s.Inventories.Where(i => i.Purchased == 0)));
        }

        private static Inventory? GetDisplayCopy(Shoppingcart cart)
        {
            return GetAvailableCopies(cart)
                .FirstOrDefault();
        }

        private static List<Inventory> GetAvailableCopies(Shoppingcart cart)
        {
            return cart.IsbnNavigation?.Inventories
                .Where(i => i.Purchased == 0)
                .OrderBy(i => i.RanksNavigation.Price)
                .ThenBy(i => i.InventoryId)
                .ToList() ?? new List<Inventory>();
        }
    }
}
