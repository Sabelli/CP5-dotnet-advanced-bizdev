namespace Biblioteca.Application.Dtos;

public class GetEmprestimoDto
{
    public int Id { get; set; }
    public int LivroId { get; set; }
    public string Livro { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataPrevistaDevolucao { get; set; }
    public DateTime? DataDevolucao { get; set; }

    public List<LinkDto> Links { get; set; } = new();
}
