using Biblioteca.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Biblioteca.API.Doc.Samples;

public class EmprestimoRequestSample : IExamplesProvider<PostEmprestimoDto>
{
    public PostEmprestimoDto GetExamples() => new() { LivroId = 1, UsuarioId = 1, DiasEmprestimo = 14 };
}

public class EmprestimoResponseSample : IExamplesProvider<GetEmprestimoDto>
{
    public GetEmprestimoDto GetExamples() => new()
    {
        Id = 1,
        LivroId = 1,
        Livro = "Dom Casmurro",
        UsuarioId = 1,
        Usuario = "Maria Silva",
        DataEmprestimo = new DateTime(2026, 10, 1),
        DataPrevistaDevolucao = new DateTime(2026, 10, 15),
        DataDevolucao = null,
        Links = new List<LinkDto> { new("/api/emprestimo/1", "self") }
    };
}

public class EmprestimoResponseListSample : IExamplesProvider<PageResponseDto<GetEmprestimoDto>>
{
    public PageResponseDto<GetEmprestimoDto> GetExamples() => new()
    {
        Items = new List<GetEmprestimoDto> { new EmprestimoResponseSample().GetExamples() },
        Deslocamento = 0,
        RegistroRetornado = 10,
        TotalRegistros = 1,
        Links = new List<LinkDto> { new("/api/emprestimo?Deslocamento=0&RegistroRetornado=10", "self") }
    };
}
