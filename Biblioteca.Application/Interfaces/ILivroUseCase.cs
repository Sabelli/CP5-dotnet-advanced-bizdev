using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Interfaces;

public interface ILivroUseCase
{
    Task<OperationResult<PageResponseDto<GetLivroDto>>> ObterTodosAsync(int Deslocamento, int RegistroRetornado);
    Task<OperationResult<GetLivroDto>> ObterUmAsync(int Id);
    Task<OperationResult<GetLivroDto>> AdicionarAsync(PostLivroDto dto);
    Task<OperationResult<GetLivroDto>> EditarAsync(int Id, PostLivroDto dto);
    Task<OperationResult<GetLivroDto>> DeletarAsync(int Id);
    Task<OperationResult<EdicaoLivroModel>> ObterPorIsbnAsync(string isbn);
}
