using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace Biblioteca.API.Doc.Samples;

public class LivroRequestSample : IExamplesProvider<PostLivroDto>
{
    public PostLivroDto GetExamples() => new()
    {
        Isbn = "9788535914849",
        Titulo = "Dom Casmurro",
        AnoPublicacao = 1899,
        AutorId = 1,
        CategoriaId = 1
    };
}

public class LivroResponseSample : IExamplesProvider<GetLivroDto>
{
    public GetLivroDto GetExamples() => new()
    {
        Id = 1,
        Isbn = "9788535914849",
        Titulo = "Dom Casmurro",
        AnoPublicacao = 1899,
        Autor = "Machado de Assis",
        Categoria = "Romance",
        Links = new List<LinkDto> { new("/api/livro/1", "self") }
    };
}

public class LivroResponseListSample : IExamplesProvider<PageResponseDto<GetLivroDto>>
{
    public PageResponseDto<GetLivroDto> GetExamples() => new()
    {
        Items = new List<GetLivroDto> { new LivroResponseSample().GetExamples() },
        Deslocamento = 0,
        RegistroRetornado = 10,
        TotalRegistros = 1,
        Links = new List<LinkDto> { new("/api/livro?Deslocamento=0&RegistroRetornado=10", "self") }
    };
}

public class LivroIsbnResponseSample : IExamplesProvider<EdicaoLivroModel>
{
    public EdicaoLivroModel GetExamples() => new()
    {
        Isbn = "9788535914849",
        Titulo = "Dom Casmurro",
        Autor = "Machado de Assis",
        AnoPublicacao = 1899
    };
}
