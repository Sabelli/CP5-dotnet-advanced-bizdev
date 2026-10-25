using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Mappers;

public static class EmprestimoMapper
{
    public static EmprestimoEntity ToEmprestimoEntity(this PostEmprestimoDto obj)
    {
        var agora = DateTime.UtcNow;
        return new EmprestimoEntity
        {
            LivroId = obj.LivroId,
            UsuarioId = obj.UsuarioId,
            DataEmprestimo = agora,
            DataPrevistaDevolucao = agora.AddDays(obj.DiasEmprestimo)
        };
    }

    public static GetEmprestimoDto ToEmprestimoDto(this EmprestimoEntity obj, IEnumerable<LinkDto>? links = null) =>
        new()
        {
            Id = obj.Id,
            LivroId = obj.LivroId,
            Livro = obj.Livro?.Titulo ?? string.Empty,
            UsuarioId = obj.UsuarioId,
            Usuario = obj.Usuario?.Nome ?? string.Empty,
            DataEmprestimo = obj.DataEmprestimo,
            DataPrevistaDevolucao = obj.DataPrevistaDevolucao,
            DataDevolucao = obj.DataDevolucao,
            Links = links?.ToList() ?? new List<LinkDto>()
        };
}
