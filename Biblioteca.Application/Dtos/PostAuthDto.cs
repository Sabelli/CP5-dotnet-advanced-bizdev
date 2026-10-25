using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostAuthDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;
}
