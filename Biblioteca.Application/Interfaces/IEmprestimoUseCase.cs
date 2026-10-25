using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Interfaces;

public interface IEmprestimoUseCase
{
    Task<OperationResult<PageResponseDto<GetEmprestimoDto>>> ObterTodosAsync(int Deslocamento, int RegistroRetornado);
    Task<OperationResult<GetEmprestimoDto>> ObterUmAsync(int Id);
    Task<OperationResult<GetEmprestimoDto>> EmprestarAsync(PostEmprestimoDto dto);
    Task<OperationResult<GetEmprestimoDto>> DevolverAsync(int Id);
}
