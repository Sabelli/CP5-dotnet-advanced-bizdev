using Biblioteca.Application.Dtos;

namespace Biblioteca.Application.Mappers;

public static class PageLinks
{
    public static List<LinkDto> Build(string rota, string recurso, int deslocamento, int registroRetornado, int totalRegistros)
    {
        var links = new List<LinkDto>
        {
            new($"{rota}?Deslocamento={deslocamento}&RegistroRetornado={registroRetornado}", "self", "GET"),
            new(rota, $"create_{recurso}", "POST")
        };

        if (deslocamento + registroRetornado < totalRegistros)
            links.Add(new($"{rota}?Deslocamento={deslocamento + registroRetornado}&RegistroRetornado={registroRetornado}", "next", "GET"));

        if (deslocamento > 0)
            links.Add(new($"{rota}?Deslocamento={Math.Max(0, deslocamento - registroRetornado)}&RegistroRetornado={registroRetornado}", "prev", "GET"));

        return links;
    }
}
