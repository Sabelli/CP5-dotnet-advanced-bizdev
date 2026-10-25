using System.Net;
using System.Net.Http.Json;
using Biblioteca.Application.Dtos;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Moq;

namespace Biblioteca.Tests.Integration;

public class ErrosCustomizadosTests : IntegracaoBase
{
    public ErrosCustomizadosTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    [Trait("Integracao", "Erros")]
    public async Task ObterUmInexistente_DeveRetornar404ComMensagem()
    {
        // Act
        var response = await Client.GetAsync($"api/autor/{int.MaxValue}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Não foi encontrado autor", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    [Trait("Integracao", "Erros")]
    public async Task ListarAlemDoFim_DeveRetornar204()
    {
        // Act
        var response = await Client.GetAsync("api/livro?Deslocamento=100000000&RegistroRetornado=10");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Erros")]
    public async Task CriarLivroComAutorInexistente_DeveRetornar404()
    {
        // Arrange
        var categoria = await CriarCategoriaAsync();

        // Act
        var response = await Client.PostAsJsonAsync("api/livro", new PostLivroDto
        {
            Isbn = "9780000000033", Titulo = "Sem Autor", AnoPublicacao = 2000, AutorId = int.MaxValue, CategoriaId = categoria.Id
        });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Erros")]
    public async Task CriarAutorInvalido_DeveRetornar400()
    {
        // Act - Nome menor que o mínimo de 3 caracteres
        var response = await Client.PostAsJsonAsync("api/autor", new PostAutorDto { Nome = "A" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Erros")]
    public async Task EmprestarLivroJaEmprestado_DeveRetornar409()
    {
        // Arrange
        var autor = await CriarAutorAsync();
        var categoria = await CriarCategoriaAsync();
        var livro = await CriarLivroAsync(autor.Id, categoria.Id);
        var dto = new PostEmprestimoDto { LivroId = livro.Id, UsuarioId = ObterUsuarioId(EmailAdmin) };

        var primeiro = await Client.PostAsJsonAsync("api/emprestimo", dto);
        RegistrarEmprestimo((await primeiro.Content.ReadFromJsonAsync<GetEmprestimoDto>(Json))!.Id);

        // Act
        var segundo = await Client.PostAsJsonAsync("api/emprestimo", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, primeiro.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segundo.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Erros")]
    public async Task DevolverDuasVezes_SegundaDeveRetornar409()
    {
        // Arrange
        var autor = await CriarAutorAsync();
        var categoria = await CriarCategoriaAsync();
        var livro = await CriarLivroAsync(autor.Id, categoria.Id);
        var criado = await Client.PostAsJsonAsync("api/emprestimo", new PostEmprestimoDto { LivroId = livro.Id, UsuarioId = ObterUsuarioId(EmailAdmin) });
        var id = (await criado.Content.ReadFromJsonAsync<GetEmprestimoDto>(Json))!.Id;
        RegistrarEmprestimo(id);

        // Act
        var primeira = await Client.PutAsync($"api/emprestimo/{id}/devolver", null);
        var segunda = await Client.PutAsync($"api/emprestimo/{id}/devolver", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, primeira.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segunda.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Erros")]
    public async Task DevolverInexistente_DeveRetornar404()
    {
        var response = await Client.PutAsync($"api/emprestimo/{int.MaxValue}/devolver", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "ServicoExterno")]
    public async Task IsbnEncontrado_DeveRetornar200ComDadosDoServicoExterno()
    {
        // Arrange
        Factory.LivroExternoMock.Setup(s => s.ObterPorIsbnAsync("9788535902778"))
            .ReturnsAsync(new EdicaoLivroModel { Isbn = "9788535902778", Titulo = "Título Externo", Autor = "Autor Externo", AnoPublicacao = 2002 });

        // Act
        var response = await Client.GetAsync("api/livro/isbn/9788535902778");
        var livro = await response.Content.ReadFromJsonAsync<EdicaoLivroModel>(Json);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Título Externo", livro!.Titulo);
        Assert.Equal(2002, livro.AnoPublicacao);
    }

    [Fact]
    [Trait("Integracao", "ServicoExterno")]
    public async Task IsbnInexistente_DeveRetornar404()
    {
        // Arrange
        Factory.LivroExternoMock.Setup(s => s.ObterPorIsbnAsync("0000000000000"))
            .ThrowsAsync(new NoContentException("Não foi encontrado livro externo com o ISBN 0000000000000"));

        // Act
        var response = await Client.GetAsync("api/livro/isbn/0000000000000");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
