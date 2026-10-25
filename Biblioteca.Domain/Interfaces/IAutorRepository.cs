using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Interfaces;

public interface IAutorRepository
{
    Task<(IEnumerable<AutorEntity> data, int rowsCount)> ObterTodosAsync(int Deslocamento = 0, int RegistroRetornado = 30);
    Task<AutorEntity> ObterUmAsync(int Id);
    Task<AutorEntity> AdicionarAsync(AutorEntity entity);
    Task<AutorEntity> EditarAsync(int Id, AutorEntity entity);
    Task<AutorEntity> DeletarAsync(int Id);
}
