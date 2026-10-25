using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostLivroDto
{
    [Required(ErrorMessage = "O ISBN é obrigatório.")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "O ISBN deve ter entre 10 e 20 caracteres.")]
    public string Isbn { get; set; } = string.Empty;

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "O título deve ter entre 1 e 200 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Range(1000, 2100, ErrorMessage = "O ano de publicação deve estar entre 1000 e 2100.")]
    public int AnoPublicacao { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe um autor válido.")]
    public int AutorId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe uma categoria válida.")]
    public int CategoriaId { get; set; }
}
