namespace Biblioteca.Application.Dtos;

public class GetCategoriaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    public List<LinkDto> Links { get; set; } = new();
}
