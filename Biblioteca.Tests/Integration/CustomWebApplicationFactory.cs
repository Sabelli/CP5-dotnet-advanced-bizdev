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

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

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
