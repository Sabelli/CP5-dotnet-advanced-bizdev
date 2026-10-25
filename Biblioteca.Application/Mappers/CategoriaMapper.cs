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
            Links = links?.ToList() ?? new List<LinkDto>()
        };
}
