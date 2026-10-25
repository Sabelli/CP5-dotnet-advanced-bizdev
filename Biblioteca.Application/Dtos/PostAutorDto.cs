using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostAutorDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(60, ErrorMessage = "A nacionalidade deve ter no máximo 60 caracteres.")]
    public string? Nacionalidade { get; set; }
}
