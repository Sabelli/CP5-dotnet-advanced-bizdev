using System.Text.Json;
using System.Text.RegularExpressions;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Biblioteca.Infrastructure.Service;

public class LivroExternoService : ILivroExternoService
{
    private const int MaxTentativas = 3;

    private readonly ILogger<LivroExternoService> _logger;
    private readonly RestClient _restClient;

    public LivroExternoService(ILogger<LivroExternoService> logger)
    {
        _logger = logger;
        _restClient = new RestClient(new RestClientOptions("https://openlibrary.org") { Timeout = TimeSpan.FromSeconds(5) });
    }

    public async Task<EdicaoLivroModel> ObterPorIsbnAsync(string isbn)
    {
        var request = new RestRequest("api/books", Method.Get)
            .AddQueryParameter("bibkeys", $"ISBN:{isbn}")
            .AddQueryParameter("format", "json")
            .AddQueryParameter("jscmd", "data");

        var response = await ExecutarComRetryAsync(request);

        if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
        {
            _logger.LogWarning("OpenLibrary falhou para o ISBN {0}: {1}", isbn, response.StatusCode);
            throw new NoContentException($"Não foi possível consultar o ISBN {isbn} no serviço externo");
        }

        using var doc = JsonDocument.Parse(response.Content);

        if (!doc.RootElement.TryGetProperty($"ISBN:{isbn}", out var livro))
            throw new NoContentException($"Não foi encontrado livro externo com o ISBN {isbn}");

        var autor = livro.TryGetProperty("authors", out var autores) && autores.GetArrayLength() > 0
            ? autores[0].GetProperty("name").GetString()
            : null;

        int? ano = null;
        if (livro.TryGetProperty("publish_date", out var data)
            && Regex.Match(data.GetString() ?? "", @"\d{4}") is { Success: true } m)
            ano = int.Parse(m.Value);

        return new EdicaoLivroModel
        {
            Isbn = isbn,
            Titulo = livro.TryGetProperty("title", out var titulo) ? titulo.GetString() ?? string.Empty : string.Empty,
            Autor = autor,
            AnoPublicacao = ano
        };
    }

    // Retry só para falha transitória (timeout/erro de rede/5xx); 4xx não é repetido.
    private async Task<RestResponse> ExecutarComRetryAsync(RestRequest request)
    {
        RestResponse response = null!;

        for (var tentativa = 1; tentativa <= MaxTentativas; tentativa++)
        {
            response = await _restClient.ExecuteAsync(request);

            var transitoria = (int)response.StatusCode >= 500
                || response.ResponseStatus is ResponseStatus.TimedOut or ResponseStatus.Error;

            if (!transitoria || tentativa == MaxTentativas)
                break;

            _logger.LogWarning("OpenLibrary tentativa {0}/{1} falhou ({2}), tentando novamente", tentativa, MaxTentativas, response.StatusCode);
            await Task.Delay(TimeSpan.FromMilliseconds(200 * tentativa));
        }

        return response;
    }
}
