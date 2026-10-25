using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Mappers;

public static class AutorMapper
{
    public static AutorEntity ToAutorEntity(this PostAutorDto obj) =>
        new()
        {
            Nome = obj.Nome,
            Nacionalidade = obj.Nacionalidade
        };

    public static GetAutorDto ToAutorDto(this AutorEntity obj, IEnumerable<LinkDto>? links = null) =>
        new()
        {
            Id = obj.Id,
            Nome = obj.Nome,
            Nacionalidade = obj.Nacionalidade,
            Links = links?.ToList() ?? BuildLinks(obj.Id)
        };

    public static List<LinkDto> BuildLinks(int id) =>
        new()
        {
            new($"/api/autor/{id}", "self", "GET"),
            new($"/api/autor/{id}", "update_autor", "PUT"),
            new($"/api/autor/{id}", "delete_autor", "DELETE")
        };
}
