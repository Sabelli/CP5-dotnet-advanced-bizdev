using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Interfaces;

public interface ILivroExternoService
{
    Task<EdicaoLivroModel> ObterPorIsbnAsync(string isbn);
}
