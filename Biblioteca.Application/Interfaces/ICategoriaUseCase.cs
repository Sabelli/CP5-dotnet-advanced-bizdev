using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Interfaces;

public interface ICategoriaUseCase
{
    Task<OperationResult<PageResponseDto<GetCategoriaDto>>> ObterTodosAsync(int Deslocamento, int RegistroRetornado);
    Task<OperationResult<GetCategoriaDto>> ObterUmAsync(int Id);
    Task<OperationResult<GetCategoriaDto>> AdicionarAsync(PostCategoriaDto dto);
    Task<OperationResult<GetCategoriaDto>> EditarAsync(int Id, PostCategoriaDto dto);
    Task<OperationResult<GetCategoriaDto>> DeletarAsync(int Id);
}
