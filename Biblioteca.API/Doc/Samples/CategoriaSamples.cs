using Biblioteca.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Biblioteca.API.Doc.Samples;

public class CategoriaRequestSample : IExamplesProvider<PostCategoriaDto>
{
    public PostCategoriaDto GetExamples() => new() { Nome = "Romance" };
}

public class CategoriaResponseSample : IExamplesProvider<GetCategoriaDto>
{
    public GetCategoriaDto GetExamples() => new()
    {
        Id = 1,
        Nome = "Romance",
        Links = new List<LinkDto> { new("/api/categoria/1", "self") }
    };
}

public class CategoriaResponseListSample : IExamplesProvider<PageResponseDto<GetCategoriaDto>>
{
    public PageResponseDto<GetCategoriaDto> GetExamples() => new()
    {
        Items = new List<GetCategoriaDto> { new CategoriaResponseSample().GetExamples() },
        Deslocamento = 0,
        RegistroRetornado = 10,
        TotalRegistros = 1,
        Links = new List<LinkDto> { new("/api/categoria?Deslocamento=0&RegistroRetornado=10", "self") }
    };
}
