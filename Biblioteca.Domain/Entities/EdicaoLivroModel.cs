namespace Biblioteca.Domain.Entities;

public class EdicaoLivroModel
{
    public string Isbn { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Autor { get; set; }
    public int? AnoPublicacao { get; set; }
}
