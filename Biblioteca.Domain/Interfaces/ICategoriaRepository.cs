using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Interfaces;

public interface ICategoriaRepository
{
    Task<(IEnumerable<CategoriaEntity> data, int rowsCount)> ObterTodosAsync(int Deslocamento = 0, int RegistroRetornado = 30);
    Task<CategoriaEntity> ObterUmAsync(int Id);
    Task<CategoriaEntity> AdicionarAsync(CategoriaEntity entity);
    Task<CategoriaEntity> EditarAsync(int Id, CategoriaEntity entity);
    Task<CategoriaEntity> DeletarAsync(int Id);
}
