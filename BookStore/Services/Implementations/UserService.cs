using AutoMapper;
using BookStore.DTOs.User;
using BookStore.Exceptions;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStoreWebAPI.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        => _mapper.Map<IEnumerable<UserDto>>(await _uow.Users.GetAllAsync());

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _uow.Users.GetByIdAsync(id)
            ?? throw new NotFoundException($"User with ID {id} not found");
        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int roleNumber)
        => _mapper.Map<IEnumerable<UserDto>>(await _uow.Users.GetUsersByRoleAsync(roleNumber));

    public async Task<UserDto> UpdateUserAsync(int id, UserUpdateDto dto)
    {
        var user = await _uow.Users.GetByIdAsync(id)
            ?? throw new NotFoundException($"User with ID {id} not found");
        _mapper.Map(dto, user);
        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        if (!await _uow.Users.ExistsAsync(id))
        {
            throw new NotFoundException($"User with ID {id} not found");
        }

        await _uow.Users.DeleteAsync(id);
        await _uow.SaveChangesAsync();
    }
}
