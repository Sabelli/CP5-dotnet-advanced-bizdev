using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostEmprestimoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe um livro válido.")]
    public int LivroId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe um usuário válido.")]
    public int UsuarioId { get; set; }

    [Range(1, 90, ErrorMessage = "O prazo do empréstimo deve ser de 1 a 90 dias.")]
    public int DiasEmprestimo { get; set; } = 14;
}
