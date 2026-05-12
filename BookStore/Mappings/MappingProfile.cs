using BookStore.DTOs.Auth;
using BookStore.DTOs.User;
using BookStore.Models;
using AutoMapper;

namespace BookStore.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, User>();
            CreateMap<UserUpdateDto, User>();

        }
    }
}