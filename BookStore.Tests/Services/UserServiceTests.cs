using AutoMapper;
using BookStore.DTOs.User;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using Moq;
using Xunit;

namespace BookStore.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IUserRepository> _userRepo = new();

        private UserService CreateSvc()
        {
            _uow.Setup(u => u.Users)
                .Returns(_userRepo.Object);

            return new UserService(
                _uow.Object,
                _mapper.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            var users = new List<UserDto>
            {
                new() { UserId = 1 },
                new() { UserId = 2 }
            };

            _userRepo
                .Setup(r => r.GetAllWithRoleNameAsync())
                .ReturnsAsync(users);

            var result = await CreateSvc().GetAllUsersAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUser_ReturnsCorrectDto()
        {
            var dto = new UserDto
            {
                UserId = 1,
                UserName = "john"
            };

            _userRepo
                .Setup(r => r.GetByIdWithRoleNameAsync(1))
                .ReturnsAsync(dto);

            var result = await CreateSvc().GetUserByIdAsync(1);

            Assert.Equal("john", result.UserName);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ExistingUser_ReturnsCorrectDto()
        {
            var dto = new UserDto
            {
                UserId = 2,
                UserName = "alice"
            };

            _userRepo
                .Setup(r => r.GetUserByUsernameWithRoleNameAsync("alice"))
                .ReturnsAsync(dto);

            var result = await CreateSvc().GetUserByUsernameAsync("alice");

            Assert.Equal(2, result.UserId);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsTrue()
        {
            var user = new User
            {
                UserId = 5,
                UserName = "todelete"
            };

            _userRepo
                .Setup(r => r.GetUserByUsernameAsync("todelete"))
                .ReturnsAsync(user);

            _userRepo
                .Setup(r => r.DeleteAsync(5))
                .Returns(Task.CompletedTask);

            _uow
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await CreateSvc().DeleteUserAsync("todelete");

            Assert.True(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            _userRepo
                .Setup(r => r.GetByIdWithRoleNameAsync(99))
                .ReturnsAsync((UserDto?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                CreateSvc().GetUserByIdAsync(99));
        }

        [Fact]
        public async Task GetUserByUsernameAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            _userRepo
                .Setup(r => r.GetUserByUsernameWithRoleNameAsync("unknown"))
                .ReturnsAsync((UserDto?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                CreateSvc().GetUserByUsernameAsync("unknown"));
        }

        [Fact]
        public async Task GetUsersByRoleAsync_RoleNotFound_ThrowsKeyNotFoundException()
        {
            _userRepo
                .Setup(r => r.RoleNameExistsAsync("Ghost"))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                CreateSvc().GetUsersByRoleAsync("Ghost"));
        }

        [Fact]
        public async Task DeleteUserAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            _userRepo
                .Setup(r => r.GetUserByUsernameAsync("nobody"))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                CreateSvc().DeleteUserAsync("nobody"));
        }
    }
}