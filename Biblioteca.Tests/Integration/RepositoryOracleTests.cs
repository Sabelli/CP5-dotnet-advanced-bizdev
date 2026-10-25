using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Biblioteca.Tests.Integration;

public class RepositoryOracleTests : IntegracaoBase
{
    public RepositoryOracleTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    [Trait("Integracao", "Repository")]
    public async Task Autor_CicloCompleto_AdicionarEditarObterDeletar()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IAutorRepository>();

        // Act - adicionar
        var criado = await repo.AdicionarAsync(new AutorEntity { Nome = "Autor Repo Teste", Nacionalidade = "BR" });
        RegistrarAutor(criado.Id);

        // Assert
        Assert.True(criado.Id > 0);

        // Act - editar
        await repo.EditarAsync(criado.Id, new AutorEntity { Nome = "Autor Repo Editado", Nacionalidade = "PT" });
        var lido = await repo.ObterUmAsync(criado.Id);

        // Assert
        Assert.Equal("Autor Repo Editado", lido.Nome);
        Assert.Equal("PT", lido.Nacionalidade);

        // Act - deletar
        await repo.DeletarAsync(criado.Id);

        // Assert - erro customizado lançado pelo repository
        await Assert.ThrowsAsync<NoContentException>(() => repo.ObterUmAsync(criado.Id));
    }

    [Fact]
    [Trait("Integracao", "Repository")]
    public async Task Autor_ObterUmInexistente_DeveLancarNoContentException()
    {
        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IAutorRepository>();

        await Assert.ThrowsAsync<NoContentException>(() => repo.ObterUmAsync(int.MaxValue));
    }

    [Fact]
    [Trait("Integracao", "Repository")]
    public async Task Autor_ObterTodosPaginado_DeveRespeitarRegistroRetornadoEInformarTotal()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IAutorRepository>();
        for (var i = 0; i < 3; i++)
            RegistrarAutor((await repo.AdicionarAsync(new AutorEntity { Nome = $"Autor Pagina {i}" })).Id);

        // Act
        var (data, total) = await repo.ObterTodosAsync(0, 2);

        // Assert
        Assert.Equal(2, data.Count());
        Assert.True(total >= 3);
    }

    [Fact]
    [Trait("Integracao", "Repository")]
    public async Task Autor_ObterTodosAlemDoFim_DeveLancarNoContentException()
    {
        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IAutorRepository>();

        await Assert.ThrowsAsync<NoContentException>(() => repo.ObterTodosAsync(100_000_000, 10));
    }

    [Fact]
    [Trait("Integracao", "Repository")]
    public async Task Livro_Adicionar_DeveTrazerAutorECategoriaNoObterUm()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var autores = scope.ServiceProvider.GetRequiredService<IAutorRepository>();
        var categorias = scope.ServiceProvider.GetRequiredService<ICategoriaRepository>();
        var livros = scope.ServiceProvider.GetRequiredService<ILivroRepository>();

        var autor = await autores.AdicionarAsync(new AutorEntity { Nome = "Autor Livro Teste" });
        RegistrarAutor(autor.Id);
        var categoria = await categorias.AdicionarAsync(new CategoriaEntity { Nome = "Cat Livro Teste" });
        RegistrarCategoria(categoria.Id);

        // Act
        var livro = await livros.AdicionarAsync(new LivroEntity
        {
            Isbn = "9780000000019", Titulo = "Livro Repo", AnoPublicacao = 2001, AutorId = autor.Id, CategoriaId = categoria.Id
        });
        RegistrarLivro(livro.Id);
        var lido = await livros.ObterUmAsync(livro.Id);

        // Assert
        Assert.Equal("Autor Livro Teste", lido.Autor?.Nome);
        Assert.Equal("Cat Livro Teste", lido.Categoria?.Nome);
    }

    [Fact]
    [Trait("Integracao", "Repository")]
    public async Task Livro_IsbnExiste_DeveIgnorarOProprioIdQuandoInformado()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var autores = scope.ServiceProvider.GetRequiredService<IAutorRepository>();
        var categorias = scope.ServiceProvider.GetRequiredService<ICategoriaRepository>();
        var livros = scope.ServiceProvider.GetRequiredService<ILivroRepository>();

        var autor = await autores.AdicionarAsync(new AutorEntity { Nome = "Autor Isbn Teste" });
        RegistrarAutor(autor.Id);
        var categoria = await categorias.AdicionarAsync(new CategoriaEntity { Nome = "Cat Isbn Teste" });
        RegistrarCategoria(categoria.Id);
        var livro = await livros.AdicionarAsync(new LivroEntity
        {
            Isbn = "9780000000028", Titulo = "Livro Isbn", AnoPublicacao = 2001, AutorId = autor.Id, CategoriaId = categoria.Id
        });
        RegistrarLivro(livro.Id);

        // Act / Assert
        Assert.True(await livros.IsbnExisteAsync("9780000000028"));
        Assert.False(await livros.IsbnExisteAsync("9780000000028", livro.Id));
        Assert.False(await livros.IsbnExisteAsync("9780000000999"));
    }

    [Fact]
    [Trait("Integracao", "Repository")]
    public async Task Emprestimo_LivroEmprestadoAsync_RefleteEmprestimoAbertoEDevolucao()
    {
        // Arrange
        var autor = await CriarAutorAsync();
        var categoria = await CriarCategoriaAsync();
        var livro = await CriarLivroAsync(autor.Id, categoria.Id);
        var usuarioId = ObterUsuarioId(EmailAdmin);

        using var scope = Factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IEmprestimoRepository>();
        Assert.False(await repo.LivroEmprestadoAsync(livro.Id));

        // Act
        var emprestimo = await repo.AdicionarAsync(new EmprestimoEntity
        {
            LivroId = livro.Id, UsuarioId = usuarioId, DataEmprestimo = DateTime.UtcNow, DataPrevistaDevolucao = DateTime.UtcNow.AddDays(7)
        });
        RegistrarEmprestimo(emprestimo.Id);

        // Assert
        Assert.True(await repo.LivroEmprestadoAsync(livro.Id));

        // Act
        await repo.DevolverAsync(emprestimo.Id);

        // Assert
        Assert.False(await repo.LivroEmprestadoAsync(livro.Id));
    }
}
