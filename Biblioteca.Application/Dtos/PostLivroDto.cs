using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostLivroDto
{
    [Required]
    [StringLength(20, MinimumLength = 10)]
    public string Isbn { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Titulo { get; set; } = string.Empty;

    [Range(1000, 2100)]
    public int AnoPublicacao { get; set; }

    [Range(1, int.MaxValue)]
    public int AutorId { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoriaId { get; set; }
}
