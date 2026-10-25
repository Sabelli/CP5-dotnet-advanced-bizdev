namespace Biblioteca.Domain.Entities;

public class LivroEntity
{
    public int Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public int AnoPublicacao { get; set; }

    public int AutorId { get; set; }
    public AutorEntity? Autor { get; set; }

    public int CategoriaId { get; set; }
    public CategoriaEntity? Categoria { get; set; }
}
