using System.Net;
using System.Net.Http.Json;
using Biblioteca.Application.Dtos;

namespace Biblioteca.Tests.Integration;

public class AutenticacaoTests : IntegracaoBase
{
    public AutenticacaoTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    [Trait("Integracao", "Auth")]
    public async Task SemToken_DeveRetornar401()
    {
        // Arrange
        using var anonimo = Factory.CreateClient();

        // Act
        var response = await anonimo.GetAsync("api/autor");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Auth")]
    public async Task TokenInvalido_DeveRetornar401()
    {
        // Arrange
        using var cliente = Factory.CreateClient();
        cliente.DefaultRequestHeaders.Authorization = new("Bearer", "token.invalido.aqui");

        // Act
        var response = await cliente.GetAsync("api/autor");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Auth")]
    public async Task Leitor_NaoPodeCriar_DeveRetornar403()
    {
        // Arrange
        using var leitor = Factory.CreateClient();
        await AutenticarAsync(leitor, EmailLeitor, "leitor");

        // Act
        var response = await leitor.PostAsJsonAsync("api/autor", new PostAutorDto { Nome = "Nao Deve Criar" });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Auth")]
    public async Task Leitor_PodeLer_DeveRetornarOkOu204()
    {
        // Arrange
        await CriarAutorAsync();
        using var leitor = Factory.CreateClient();
        await AutenticarAsync(leitor, EmailLeitor, "leitor");

        // Act
        var response = await leitor.GetAsync("api/autor");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Auth")]
    public async Task Admin_PodeCriar_DeveRetornar201()
    {
        // Act (Client já autenticado como admin)
        var autor = await CriarAutorAsync();

        // Assert
        Assert.True(autor.Id > 0);
    }

    [Fact]
    [Trait("Integracao", "Auth")]
    public async Task Login_SenhaErrada_DeveRetornar404()
    {
        // Arrange
        using var anonimo = Factory.CreateClient();

        // Act
        var response = await anonimo.PostAsJsonAsync("api/auth", new PostAuthDto { Email = EmailAdmin, Senha = "errada" });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Auth")]
    public async Task Cadastro_EmailDuplicado_DeveRetornar409()
    {
        // Arrange
        using var anonimo = Factory.CreateClient();
        var dto = new PostUserDto { Nome = "Admin", Email = EmailAdmin, Password = Senha, ConfirmPassword = Senha, Role = "admin" };

        // Act
        var response = await anonimo.PostAsJsonAsync("api/user", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    [Trait("Integracao", "Auth")]
    public async Task Cadastro_SenhasDiferentes_DeveRetornar400()
    {
        // Arrange
        using var anonimo = Factory.CreateClient();
        var dto = new PostUserDto { Nome = "Fulano", Email = "fulano.invalido@teste.com", Password = "Senha123", ConfirmPassword = "Outra123", Role = "leitor" };

        // Act
        var response = await anonimo.PostAsJsonAsync("api/user", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
