namespace Biblioteca.Domain.Entities;

public class CategoriaEntity
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    public ICollection<LivroEntity> Livros { get; set; } = new List<LivroEntity>();
}
