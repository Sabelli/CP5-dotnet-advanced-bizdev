using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostCategoriaDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 60 caracteres.")]
    public string Nome { get; set; } = string.Empty;
}
