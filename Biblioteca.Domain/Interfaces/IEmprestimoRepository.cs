using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Interfaces;

public interface IEmprestimoRepository
{
    Task<(IEnumerable<EmprestimoEntity> data, int rowsCount)> ObterTodosAsync(int Deslocamento = 0, int RegistroRetornado = 30);
    Task<EmprestimoEntity> ObterUmAsync(int Id);
    Task<bool> LivroEmprestadoAsync(int LivroId);
    Task<EmprestimoEntity> AdicionarAsync(EmprestimoEntity entity);
    Task<EmprestimoEntity> DevolverAsync(int Id);
}
