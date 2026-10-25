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
            Links = links?.ToList() ?? new List<LinkDto>()
        };
}
