using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Interfaces;

public interface IUserUseCase
{
    Task<OperationResult<GetUserDto>> AdicionarUserAsync(PostUserDto dto);
    Task<OperationResult<GetAuthDto>> AutenticaUserAsync(PostAuthDto dto);
}
