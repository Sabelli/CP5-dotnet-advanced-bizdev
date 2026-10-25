namespace Biblioteca.Application.Dtos;

public class GetAutorDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Nacionalidade { get; set; }

    public List<LinkDto> Links { get; set; } = new();
}
