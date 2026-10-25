using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Application.Dtos;

public class PostEmprestimoDto
{
    [Range(1, int.MaxValue)]
    public int LivroId { get; set; }

    [Range(1, int.MaxValue)]
    public int UsuarioId { get; set; }

    [Range(1, 90)]
    public int DiasEmprestimo { get; set; } = 14;
}
