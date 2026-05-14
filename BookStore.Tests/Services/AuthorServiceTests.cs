using AutoMapper;
using BookStore.DTOs.Author;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using BookStore.Tests.Services;
using Moq;


namespace BookStore.Tests.Services
{
	public class AuthorServiceTests
	{
		private readonly Mock<IUnitOfWork> _uowMock;
		private readonly Mock<IAuthorRepository> _authorRepoMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly AuthorService _service;

		public AuthorServiceTests()
		{
			_uowMock = new Mock<IUnitOfWork>();
			_authorRepoMock = new Mock<IAuthorRepository>();
			_mapperMock = new Mock<IMapper>();

			_uowMock.Setup(u => u.Authors).Returns(_authorRepoMock.Object);

			_service = new AuthorService(_uowMock.Object, _mapperMock.Object);
		}

		// Positive 1: GetAll returns DTOs
		[Fact]
		public async Task GetAllAuthorsAsync_ReturnsDtos_WhenAuthorsExist()
		{
			var author = new Author { AuthorId = 1, FirstName = "Alice", LastName = "Smith" };
			var dto = new AuthorDto { AuthorId = 1, FirstName = "Alice", LastName = "Smith" };

			_authorRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Author> { author });
			_mapperMock.Setup(m => m.Map<IEnumerable<AuthorDto>>(It.IsAny<IEnumerable<Author>>()))
					   .Returns(new List<AuthorDto> { dto });

			var result = await _service.GetAllAuthorsAsync();

			Assert.NotNull(result);
			var list = new List<AuthorDto>(result);
			Assert.Single(list);
			Assert.Equal("Alice", list[0].FirstName);
		}

		// Positive 2: Create adds and saves
		[Fact]
		public async Task CreateAuthorAsync_AddsAndSaves()
		{
			var createDto = new AuthorCreateDto { FirstName = "Bob", LastName = "Lee" };
			var authorEntity = new Author { AuthorId = 2, FirstName = "Bob", LastName = "Lee" };
			var outDto = new AuthorDto { AuthorId = 2, FirstName = "Bob", LastName = "Lee" };

			_mapperMock.Setup(m => m.Map<Author>(createDto)).Returns(authorEntity);
			_mapperMock.Setup(m => m.Map<AuthorDto>(It.IsAny<Author>())).Returns(outDto);

			_authorRepoMock.Setup(r => r.AddAsync(authorEntity)).Returns(Task.CompletedTask);
			_uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

			var result = await _service.CreateAuthorAsync(createDto);

			Assert.NotNull(result);
			Assert.Equal("Bob", result.FirstName);
			_authorRepoMock.Verify(r => r.AddAsync(authorEntity), Times.Once);
			_uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
		}

		// Positive 3: Search returns DTOs when matches found
		[Fact]
		public async Task SearchAuthorsAsync_ReturnsDtos_WhenMatchesFound()
		{
			var author = new Author { AuthorId = 3, FirstName = "Carol", LastName = "Jones" };
			var dto = new AuthorDto { AuthorId = 3, FirstName = "Carol", LastName = "Jones" };

			_authorRepoMock.Setup(r => r.SearchAuthorsAsync("Carol")).ReturnsAsync(new List<Author> { author });
			_mapperMock.Setup(m => m.Map<IEnumerable<AuthorDto>>(It.IsAny<IEnumerable<Author>>()))
					   .Returns(new List<AuthorDto> { dto });

			var result = await _service.SearchAuthorsAsync("Carol");

			Assert.NotNull(result);
			var list = new List<AuthorDto>(result);
			Assert.Single(list);
			Assert.Equal("Carol", list[0].FirstName);
		}

		// Positive 4: Update updates and returns DTO when author exists
		[Fact]
		public async Task UpdateAuthorAsync_UpdatesAndReturnsDto_WhenAuthorExists()
		{
			var id = 4;
			var existing = new Author { AuthorId = id, FirstName = "Old", LastName = "Name" };
			var updateDto = new AuthorCreateDto { FirstName = "New", LastName = "Name" };
			var outDto = new AuthorDto { AuthorId = id, FirstName = "New", LastName = "Name" };

			_authorRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
			_mapperMock.Setup(m => m.Map<AuthorDto>(It.IsAny<Author>())).Returns(outDto);
			_authorRepoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
			_uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

			var result = await _service.UpdateAuthorAsync(id, updateDto);

			Assert.NotNull(result);
			Assert.Equal("New", result.FirstName);
			_authorRepoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
			_uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
		}

		// Negative 1: Update throws NotFound when missing
		[Fact]
		public async Task UpdateAuthorAsync_ThrowsNotFound_WhenMissing()
		{
			_authorRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Author?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAuthorAsync(99, new AuthorCreateDto()));
		}

		// Negative 2: Delete throws NotFound when missing
		[Fact]
		public async Task DeleteAuthorAsync_ThrowsNotFound_WhenMissing()
		{
			_authorRepoMock.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

			await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAuthorAsync(99));
		}

		// Negative 3: Create propagates when repository Add fails
		[Fact]
		public async Task CreateAuthorAsync_Throws_WhenRepositoryAddFails()
		{
			var createDto = new AuthorCreateDto { FirstName = "Err", LastName = "Case" };
			var authorEntity = new Author { FirstName = "Err", LastName = "Case" };

			_mapperMock.Setup(m => m.Map<Author>(createDto)).Returns(authorEntity);
			_authorRepoMock.Setup(r => r.AddAsync(authorEntity)).ThrowsAsync(new InvalidOperationException("DB add failed"));

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAuthorAsync(createDto));
		}

		// Negative 4: Update propagates when SaveChanges fails
		[Fact]
		public async Task UpdateAuthorAsync_Throws_WhenSaveChangesFails()
		{
			var id = 5;
			var existing = new Author { AuthorId = id, FirstName = "Will", LastName = "Fail" };
			var updateDto = new AuthorCreateDto { FirstName = "Will", LastName = "Fail" };

			_authorRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
			_authorRepoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
			_uowMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new InvalidOperationException("Save failed"));

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAuthorAsync(id, updateDto));
		}
	}
}