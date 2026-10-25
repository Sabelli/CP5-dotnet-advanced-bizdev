namespace Biblioteca.Application.Dtos;

public class GetLivroDto
{
    public int Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public int AnoPublicacao { get; set; }
    public string Autor { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;

    public List<LinkDto> Links { get; set; } = new();
}
