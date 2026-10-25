using Biblioteca.Application.Interfaces;
using Biblioteca.Application.Services;
using Biblioteca.Application.UseCases;
using Biblioteca.Domain.Interfaces;
using Biblioteca.Infrastructure.Data;
using Biblioteca.Infrastructure.Data.Repositories;
using Biblioteca.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Biblioteca.IoC;

public static class Bootstrap
{
    public static void AddIoC(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseOracle(
                configuration.GetConnectionString("Oracle"),
                o => o.MigrationsHistoryTable("__BibliotecaMigrationsHistory")));

        services.AddTransient<IAutorRepository, AutorRepository>();
        services.AddTransient<ICategoriaRepository, CategoriaRepository>();
        services.AddTransient<ILivroRepository, LivroRepository>();
        services.AddTransient<IEmprestimoRepository, EmprestimoRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<ILivroExternoService, LivroExternoService>();
        services.AddTransient<ITokenService, TokenService>();

        services.AddTransient<IAutorUseCase, AutorUseCase>();
        services.AddTransient<ICategoriaUseCase, CategoriaUseCase>();
        services.AddTransient<ILivroUseCase, LivroUseCase>();
        services.AddTransient<IEmprestimoUseCase, EmprestimoUseCase>();
    }
}
