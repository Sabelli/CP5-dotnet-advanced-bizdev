using Biblioteca.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Biblioteca.API.Doc.Samples;

public class AutorRequestSample : IExamplesProvider<PostAutorDto>
{
    public PostAutorDto GetExamples() => new() { Nome = "Machado de Assis", Nacionalidade = "Brasileira" };
}

public class AutorResponseSample : IExamplesProvider<GetAutorDto>
{
    public GetAutorDto GetExamples() => new()
    {
        Id = 1,
        Nome = "Machado de Assis",
        Nacionalidade = "Brasileira",
        Links = new List<LinkDto> { new("/api/autor/1", "self") }
    };
}

public class AutorResponseListSample : IExamplesProvider<PageResponseDto<GetAutorDto>>
{
    public PageResponseDto<GetAutorDto> GetExamples() => new()
    {
        Items = new List<GetAutorDto> { new AutorResponseSample().GetExamples() },
        Deslocamento = 0,
        RegistroRetornado = 10,
        TotalRegistros = 1,
        Links = new List<LinkDto> { new("/api/autor?Deslocamento=0&RegistroRetornado=10", "self") }
    };
}
