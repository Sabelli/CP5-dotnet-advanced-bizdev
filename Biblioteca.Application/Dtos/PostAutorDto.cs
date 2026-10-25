using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostAutorDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(60)]
    public string? Nacionalidade { get; set; }
}
