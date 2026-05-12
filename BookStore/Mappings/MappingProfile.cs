using BookStore.DTOs.Author;
using BookStore.DTOs.Book;
using BookStore.Models;
using AutoMapper;

namespace BookStore.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{

			CreateMap<Book, BookDto>()
				.ForMember(d => d.CategoryName, o => o.MapFrom(s => s.CategoryNavigation != null ? s.CategoryNavigation.CatDescription : null))
				.ForMember(d => d.PublisherName, o => o.MapFrom(s => s.Publisher != null ? s.Publisher.Name : null));
			CreateMap<BookCreateDto, Book>();
			CreateMap<BookUpdateDto, Book>();

			CreateMap<Author, AuthorDto>();
			CreateMap<AuthorCreateDto, Author>();
			CreateMap<Bookauthor, BookAuthorDto>().ReverseMap();

		}
	}

}
