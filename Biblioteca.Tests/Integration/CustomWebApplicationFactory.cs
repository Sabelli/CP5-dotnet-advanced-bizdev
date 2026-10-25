using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Biblioteca.Tests.Integration;

// Sobe a API real em Development (appsettings.Development.json -> Oracle).
// Só o serviço externo (OpenLibrary) é substituído por mock, para o teste não depender da internet.
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<ILivroExternoService> LivroExternoMock { get; } = new();

    public CustomWebApplicationFactory()
    {
        // Padrão: qualquer ISBN existe na OpenLibrary. Testes específicos sobrescrevem com Setup próprio.
        LivroExternoMock.Setup(s => s.ObterPorIsbnAsync(It.IsAny<string>()))
            .ReturnsAsync((string isbn) => new EdicaoLivroModel { Isbn = isbn, Titulo = "Título Externo", AnoPublicacao = 2000 });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        // Testes compartilham o mesmo IP: limites altos para não gerar 429.
        builder.UseSetting("RateLimit:Escrita", "100000");
        builder.UseSetting("RateLimit:Auth", "100000");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ILivroExternoService>();
            services.AddSingleton(LivroExternoMock.Object);
        });
    }
}

[CollectionDefinition(Nome)]
public class IntegracaoCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    public const string Nome = "Integracao";
}
