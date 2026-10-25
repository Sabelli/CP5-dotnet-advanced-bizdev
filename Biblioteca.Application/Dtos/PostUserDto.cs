using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "As Senhas não conferem")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(admin|leitor)$", ErrorMessage = "Role deve ser admin ou leitor")]
    public string Role { get; set; } = "leitor";
}
