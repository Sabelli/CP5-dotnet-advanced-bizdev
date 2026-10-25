namespace Biblioteca.Domain.Entities;

public class AutorEntity
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Nacionalidade { get; set; }

    public ICollection<LivroEntity> Livros { get; set; } = new List<LivroEntity>();
}
