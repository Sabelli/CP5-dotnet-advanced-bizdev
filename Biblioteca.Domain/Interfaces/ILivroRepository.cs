using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Interfaces;

public interface ILivroRepository
{
    Task<(IEnumerable<LivroEntity> data, int rowsCount)> ObterTodosAsync(int Deslocamento = 0, int RegistroRetornado = 30);
    Task<LivroEntity> ObterUmAsync(int Id);
    Task<LivroEntity> AdicionarAsync(LivroEntity entity);
    Task<LivroEntity> EditarAsync(int Id, LivroEntity entity);
    Task<LivroEntity> DeletarAsync(int Id);
}
