using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Interfaces;

public interface IAutorUseCase
{
    Task<OperationResult<PageResponseDto<GetAutorDto>>> ObterTodosAsync(int Deslocamento, int RegistroRetornado);
    Task<OperationResult<GetAutorDto>> ObterUmAsync(int Id);
    Task<OperationResult<GetAutorDto>> AdicionarAsync(PostAutorDto dto);
    Task<OperationResult<GetAutorDto>> EditarAsync(int Id, PostAutorDto dto);
    Task<OperationResult<GetAutorDto>> DeletarAsync(int Id);
}
