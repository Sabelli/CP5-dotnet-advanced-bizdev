using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Mappers;

public static class LivroMapper
{
    public static LivroEntity ToLivroEntity(this PostLivroDto obj) =>
        new()
        {
            Isbn = obj.Isbn,
            Titulo = obj.Titulo!,
            AnoPublicacao = obj.AnoPublicacao!.Value,
            AutorId = obj.AutorId,
            CategoriaId = obj.CategoriaId
        };

    public static GetLivroDto ToLivroDto(this LivroEntity obj, IEnumerable<LinkDto>? links = null) =>
        new()
        {
            Id = obj.Id,
            Isbn = obj.Isbn,
            Titulo = obj.Titulo,
            AnoPublicacao = obj.AnoPublicacao,
            Autor = obj.Autor?.Nome ?? string.Empty,
            Categoria = obj.Categoria?.Nome ?? string.Empty,
            Links = links?.ToList() ?? BuildLinks(obj.Id)
        };

    public static List<LinkDto> BuildLinks(int id) =>
        new()
        {
            new($"/api/livro/{id}", "self", "GET"),
            new($"/api/livro/{id}", "update_livro", "PUT"),
            new($"/api/livro/{id}", "delete_livro", "DELETE")
        };
}
