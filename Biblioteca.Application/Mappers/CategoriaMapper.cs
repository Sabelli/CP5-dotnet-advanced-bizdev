using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Mappers;

public static class CategoriaMapper
{
    public static CategoriaEntity ToCategoriaEntity(this PostCategoriaDto obj) =>
        new() { Nome = obj.Nome };

    public static GetCategoriaDto ToCategoriaDto(this CategoriaEntity obj, IEnumerable<LinkDto>? links = null) =>
        new()
        {
            Id = obj.Id,
            Nome = obj.Nome,
            Links = links?.ToList() ?? BuildLinks(obj.Id)
        };

    public static List<LinkDto> BuildLinks(int id) =>
        new()
        {
            new($"/api/categoria/{id}", "self", "GET"),
            new($"/api/categoria/{id}", "update_categoria", "PUT"),
            new($"/api/categoria/{id}", "delete_categoria", "DELETE")
        };
}
