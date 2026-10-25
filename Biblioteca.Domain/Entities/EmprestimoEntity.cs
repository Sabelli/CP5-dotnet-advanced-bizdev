namespace Biblioteca.Domain.Entities;

public class EmprestimoEntity
{
    public int Id { get; set; }
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataPrevistaDevolucao { get; set; }
    public DateTime? DataDevolucao { get; set; }

    public int UsuarioId { get; set; }
    public UserEntity? Usuario { get; set; }

    public int LivroId { get; set; }
    public LivroEntity? Livro { get; set; }
}
