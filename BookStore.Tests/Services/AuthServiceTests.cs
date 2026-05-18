using AutoMapper;
using BookStore.DTOs.Auth;
using BookStore.DTOs.User;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using BookStore.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BookStore.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepo = new();
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IJwtService> _jwt = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IConfiguration> _config = new();

        private AuthService CreateSvc() =>
            new(
                _authRepo.Object,
                _userRepo.Object,
                _uow.Object,
                _jwt.Object,
                _mapper.Object,
                _config.Object);

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsAuthResponseDto()
        {
            var user = new User
            {
                UserId = 1,
                UserName = "john",
                RoleNumberNavigation = new Permrole
                {
                    PermRole1 = "Admin"
                }
            };

            _authRepo
                .Setup(r => r.ValidateUserAsync("john", "pass"))
                .ReturnsAsync(user);

            _jwt
                .Setup(j => j.GenerateToken(user))
                .Returns("jwt-token");

            var result = await CreateSvc().LoginAsync(new LoginDto
            {
                UserName = "john",
                Password = "pass"
            });

            Assert.NotNull(result);
            Assert.Equal("jwt-token", result.Token);
            Assert.Equal("Admin", result.Role);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsCorrectUserId()
        {
            var user = new User
            {
                UserId = 5,
                UserName = "alice",
                RoleNumberNavigation = new Permrole
                {
                    PermRole1 = "User"
                }
            };

            _authRepo
                .Setup(r => r.ValidateUserAsync("alice", "pass"))
                .ReturnsAsync(user);

            _jwt
                .Setup(j => j.GenerateToken(user))
                .Returns("tok");

            var result = await CreateSvc().LoginAsync(new LoginDto
            {
                UserName = "alice",
                Password = "pass"
            });

            Assert.Equal(5, result!.UserId);
        }

        [Fact]
        public async Task LoginAsync_UserWithNoRoleNavigation_ReturnsGuestRole()
        {
            var user = new User
            {
                UserId = 2,
                UserName = "jane",
                RoleNumberNavigation = null
            };

            _authRepo
                .Setup(r => r.ValidateUserAsync("jane", "pass"))
                .ReturnsAsync(user);

            _jwt
                .Setup(j => j.GenerateToken(user))
                .Returns("tok");

            var result = await CreateSvc().LoginAsync(new LoginDto
            {
                UserName = "jane",
                Password = "pass"
            });

            Assert.Equal("Guest", result!.Role);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsCreatedUserDto()
        {
            var dto = new RegisterDto
            {
                UserName = "newuser",
                Password = "pass"
            };

            var user = new User
            {
                UserId = 3,
                UserName = "newuser"
            };

            var userDto = new UserDto
            {
                UserId = 3,
                UserName = "newuser"
            };

            _authRepo
                .Setup(r => r.UserExistsAsync("newuser"))
                .ReturnsAsync(false);

            _mapper
                .Setup(m => m.Map<User>(dto))
                .Returns(user);

            _userRepo
                .Setup(r => r.AddAsync(user))
                .Returns(Task.CompletedTask);

            _uow
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _userRepo
                .Setup(r => r.GetByIdWithRoleNameAsync(3))
                .ReturnsAsync(userDto);

            var result = await CreateSvc().RegisterAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("newuser", result.UserName);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsNull()
        {
            _authRepo
                .Setup(r => r.ValidateUserAsync("bad", "bad"))
                .ReturnsAsync((User?)null);

            var result = await CreateSvc().LoginAsync(new LoginDto
            {
                UserName = "bad",
                Password = "bad"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_DuplicateUsername_ReturnsNull()
        {
            _authRepo
                .Setup(r => r.UserExistsAsync("existing"))
                .ReturnsAsync(true);

            var result = await CreateSvc().RegisterAsync(new RegisterDto
            {
                UserName = "existing",
                Password = "pass"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_UserNotFoundAfterSave_ThrowsKeyNotFoundException()
        {
            var dto = new RegisterDto
            {
                UserName = "ghost",
                Password = "pass"
            };

            var user = new User
            {
                UserId = 99,
                UserName = "ghost"
            };

            _authRepo
                .Setup(r => r.UserExistsAsync("ghost"))
                .ReturnsAsync(false);

            _mapper
                .Setup(m => m.Map<User>(dto))
                .Returns(user);

            _userRepo
                .Setup(r => r.AddAsync(user))
                .Returns(Task.CompletedTask);

            _uow
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _userRepo
                .Setup(r => r.GetByIdWithRoleNameAsync(99))
                .ReturnsAsync((UserDto?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                CreateSvc().RegisterAsync(dto));
        }

        [Fact]
        public async Task RegisterAsync_NewUser_AssignsDefaultRoleNumberOne()
        {
            var dto = new RegisterDto
            {
                UserName = "newuser2",
                Password = "pass"
            };

            User? captured = null;

            var user = new User
            {
                UserId = 10,
                UserName = "newuser2"
            };

            _authRepo
                .Setup(r => r.UserExistsAsync("newuser2"))
                .ReturnsAsync(false);

            _mapper
                .Setup(m => m.Map<User>(dto))
                .Returns(user);

            _userRepo
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => captured = u)
                .Returns(Task.CompletedTask);

            _uow
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _userRepo
                .Setup(r => r.GetByIdWithRoleNameAsync(10))
                .ReturnsAsync(new UserDto
                {
                    UserId = 10
                });

            await CreateSvc().RegisterAsync(dto);

            Assert.Equal(2, captured?.RoleNumber);
        }
    }
}