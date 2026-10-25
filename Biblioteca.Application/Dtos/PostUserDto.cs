using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostUserDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "As Senhas não conferem")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "A role é obrigatória.")]
    [RegularExpression("^(admin|leitor)$", ErrorMessage = "Role deve ser admin ou leitor")]
    public string Role { get; set; } = "leitor";
}
