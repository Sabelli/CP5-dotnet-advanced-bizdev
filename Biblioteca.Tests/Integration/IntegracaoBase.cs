using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Biblioteca.Application.Dtos;
using Biblioteca.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Biblioteca.Tests.Integration;

// Base dos testes de integração: Oracle real, cliente HTTP autenticado e limpeza só das linhas criadas pelo teste.
[Collection(IntegracaoCollection.Nome)]
public abstract class IntegracaoBase : IAsyncLifetime
{
    protected static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    protected const string Senha = "Senha123";
    protected const string EmailAdmin = "admin.teste@biblioteca.com";
    protected const string EmailLeitor = "leitor.teste@biblioteca.com";

    protected readonly CustomWebApplicationFactory Factory;
    protected HttpClient Client { get; private set; } = null!;

    // Linhas criadas pelo teste, removidas em DisposeAsync (ordem: empréstimo -> livro -> autor/categoria).
    private readonly List<int> _emprestimos = new();
    private readonly List<int> _livros = new();
    private readonly List<int> _autores = new();
    private readonly List<int> _categorias = new();

    protected IntegracaoBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
    }

    public async Task InitializeAsync()
    {
        Client = Factory.CreateClient();
        await AutenticarAsync(Client, EmailAdmin, "admin");
    }

    public async Task DisposeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        foreach (var id in _emprestimos) context.Emprestimo.Where(x => x.Id == id).ExecuteDelete();
        foreach (var id in _livros) context.Livro.Where(x => x.Id == id).ExecuteDelete();
        foreach (var id in _autores) context.Autor.Where(x => x.Id == id).ExecuteDelete();
        foreach (var id in _categorias) context.Categoria.Where(x => x.Id == id).ExecuteDelete();

        Client.Dispose();
        await Task.CompletedTask;
    }

    // Cadastra o usuário (409 se já existir: idempotente), faz login e coloca o Bearer no cliente.
    protected async Task AutenticarAsync(HttpClient client, string email, string role)
    {
        await client.PostAsJsonAsync("api/user", new PostUserDto
        {
            Nome = role, Email = email, Password = Senha, ConfirmPassword = Senha, Role = role
        });

        var login = await client.PostAsJsonAsync("api/auth", new PostAuthDto { Email = email, Senha = Senha });
        login.EnsureSuccessStatusCode();
        var auth = await login.Content.ReadFromJsonAsync<GetAuthDto>(Json);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Jwt);
    }

    protected async Task<GetAutorDto> CriarAutorAsync(string? nome = null)
    {
        var response = await Client.PostAsJsonAsync("api/autor", new PostAutorDto { Nome = nome ?? $"Autor Teste {Guid.NewGuid():N}"[..30] });
        response.EnsureSuccessStatusCode();
        var autor = (await response.Content.ReadFromJsonAsync<GetAutorDto>(Json))!;
        _autores.Add(autor.Id);
        return autor;
    }

    protected async Task<GetCategoriaDto> CriarCategoriaAsync()
    {
        var response = await Client.PostAsJsonAsync("api/categoria", new PostCategoriaDto { Nome = $"Cat {Guid.NewGuid():N}"[..20] });
        response.EnsureSuccessStatusCode();
        var categoria = (await response.Content.ReadFromJsonAsync<GetCategoriaDto>(Json))!;
        _categorias.Add(categoria.Id);
        return categoria;
    }

    protected async Task<GetLivroDto> CriarLivroAsync(int autorId, int categoriaId)
    {
        var response = await Client.PostAsJsonAsync("api/livro", new PostLivroDto
        {
            Isbn = "9780000000002", Titulo = "Livro de Teste", AnoPublicacao = 2000, AutorId = autorId, CategoriaId = categoriaId
        });
        response.EnsureSuccessStatusCode();
        var livro = (await response.Content.ReadFromJsonAsync<GetLivroDto>(Json))!;
        _livros.Add(livro.Id);
        return livro;
    }

    protected void RegistrarEmprestimo(int id) => _emprestimos.Add(id);
    protected void RegistrarLivro(int id) => _livros.Add(id);
    protected void RegistrarAutor(int id) => _autores.Add(id);
    protected void RegistrarCategoria(int id) => _categorias.Add(id);

    protected int ObterUsuarioId(string email)
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        return context.Usuario.Single(x => x.Email == email).Id;
    }
}
