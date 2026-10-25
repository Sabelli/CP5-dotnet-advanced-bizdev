using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Mappers;

public static class UserMapper
{
    public static UserEntity ToUserEntity(this PostUserDto obj, string passwordHash) =>
        new()
        {
            Nome = obj.Nome,
            Email = obj.Email,
            PasswordHash = passwordHash,
            Role = obj.Role
        };

    public static GetUserDto ToUserDto(this UserEntity obj) =>
        new()
        {
            Id = obj.Id,
            Nome = obj.Nome,
            Email = obj.Email,
            Role = obj.Role
        };
}
