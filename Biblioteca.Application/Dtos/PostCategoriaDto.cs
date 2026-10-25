using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostCategoriaDto
{
    [Required]
    [StringLength(60, MinimumLength = 3)]
    public string Nome { get; set; } = string.Empty;
}
