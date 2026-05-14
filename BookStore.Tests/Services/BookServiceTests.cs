using AutoMapper;
using BookStore.DTOs.Book;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Tests.Services
{
	public class BookServiceTests
	{
		private readonly Mock<IUnitOfWork> _uowMock;
		private readonly Mock<IBookRepository> _bookRepoMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly BookService _service;

		public BookServiceTests()
		{
			_uowMock = new Mock<IUnitOfWork>();
			_bookRepoMock = new Mock<IBookRepository>();
			_mapperMock = new Mock<IMapper>();

			_uowMock.Setup(u => u.Books).Returns(_bookRepoMock.Object);

			_service = new BookService(_uowMock.Object, _mapperMock.Object);
		}

		[Fact]
		public async Task GetAllBooksAsync_ReturnsDTOs_WhenBooksExist()
		{
			var book = new Book { Isbn = "ISBN-1", Title = "Title1", Description = "d", PublisherId = 1 };
			var dto = new BookDto { Isbn = "ISBN-1", Title = "Title1", Description = "d", PublisherId = 1 };

			_bookRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Book> { book });
			_mapperMock.Setup(m => m.Map<IEnumerable<BookDto>>(It.IsAny<IEnumerable<Book>>()))
					   .Returns(new List<BookDto> { dto });

			var result = await _service.GetAllBooksAsync();

			Assert.NotNull(result);
			var list = new List<BookDto>(result);
			Assert.Single(list);
			Assert.Equal("Title1", list[0].Title);
		}

		[Fact]
		public async Task CreateBookAsync_AddsAndSaves()
		{
			var createDto = new BookCreateDto { Isbn = "ISBN-2", Title = "New", Description = "desc", PublisherId = 2 };
			var bookEntity = new Book { Isbn = "ISBN-2", Title = "New", Description = "desc", PublisherId = 2 };
			var outDto = new BookDto { Isbn = "ISBN-2", Title = "New", Description = "desc", PublisherId = 2 };

			_mapperMock.Setup(m => m.Map<Book>(createDto)).Returns(bookEntity);
			_mapperMock.Setup(m => m.Map<BookDto>(It.IsAny<Book>())).Returns(outDto);

			_bookRepoMock.Setup(r => r.AddAsync(bookEntity)).Returns(Task.CompletedTask);
			_uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

			var result = await _service.CreateBookAsync(createDto);

			Assert.NotNull(result);
			Assert.Equal("ISBN-2", result.Isbn);
			_bookRepoMock.Verify(r => r.AddAsync(bookEntity), Times.Once);
			_uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task GetBookByIsbnAsync_ReturnsDto_WhenBookExists()
		{
			var book = new Book { Isbn = "ISBN-3", Title = "Found", Description = "d", PublisherId = 1 };
			var dto = new BookDto { Isbn = "ISBN-3", Title = "Found", Description = "d", PublisherId = 1 };

			_bookRepoMock.Setup(r => r.GetBookWithDetailsAsync("ISBN-3")).ReturnsAsync(book);
			_mapperMock.Setup(m => m.Map<BookDto>(It.IsAny<Book>())).Returns(dto);

			var result = await _service.GetBookByIsbnAsync("ISBN-3");

			Assert.NotNull(result);
			Assert.Equal("Found", result.Title);
		}

		[Fact]
		public async Task UpdateBookAsync_UpdatesAndReturnsDto_WhenBookExists()
		{
			var isbn = "ISBN-4";
			var existing = new Book { Isbn = isbn, Title = "Before", PublisherId = 1 };
			var updateDto = new BookUpdateDto { Title = "After" };
			var outDto = new BookDto { Isbn = isbn, Title = "After", PublisherId = 1 };

			_bookRepoMock.Setup(r => r.GetByIdAsync(isbn)).ReturnsAsync(existing);
			_mapperMock.Setup(m => m.Map<BookDto>(It.IsAny<Book>())).Returns(outDto);
			_bookRepoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
			_uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

			var result = await _service.UpdateBookAsync(isbn, updateDto);

			Assert.NotNull(result);
			Assert.Equal("After", result.Title);
			_bookRepoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
			_uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task GetBookByIsbnAsync_ThrowsNotFound_WhenMissing()
		{
			_bookRepoMock.Setup(r => r.GetBookWithDetailsAsync("missing")).ReturnsAsync((Book?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => _service.GetBookByIsbnAsync("missing"));
		}

		[Fact]
		public async Task UpdateBookAsync_ThrowsNotFound_WhenMissing()
		{
			_bookRepoMock.Setup(r => r.GetByIdAsync("no")).ReturnsAsync((Book?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateBookAsync("no", new BookUpdateDto()));
		}

		[Fact]
		public async Task DeleteBookAsync_ThrowsNotFound_WhenNotExists()
		{
			_bookRepoMock.Setup(r => r.ExistsAsync("no")).ReturnsAsync(false);

			await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteBookAsync("no"));
		}

		[Fact]
		public async Task CreateBookAsync_Throws_WhenRepositoryAddFails()
		{
			var createDto = new BookCreateDto { Isbn = "ISBN-err", Title = "Err", Description = "desc", PublisherId = 2 };
			var bookEntity = new Book { Isbn = "ISBN-err", Title = "Err", Description = "desc", PublisherId = 2 };

			_mapperMock.Setup(m => m.Map<Book>(createDto)).Returns(bookEntity);
			_bookRepoMock.Setup(r => r.AddAsync(bookEntity)).ThrowsAsync(new InvalidOperationException("DB add failed"));

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateBookAsync(createDto));
		}
	}
}