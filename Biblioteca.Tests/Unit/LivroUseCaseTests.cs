using Biblioteca.Application.Dtos;
using Biblioteca.Application.UseCases;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Biblioteca.Tests.Unit;

public class LivroUseCaseTests
{
    private readonly Mock<ILivroRepository> _livroRepository = new();
    private readonly Mock<IAutorRepository> _autorRepository = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly Mock<ILivroExternoService> _livroExterno = new();
    private readonly LivroUseCase _useCase;

    private static PostLivroDto NovoLivro() => new()
    {
        Isbn = "9788535902778",
        Titulo = "Dom Casmurro",
        AnoPublicacao = 1899,
        AutorId = 1,
        CategoriaId = 2
    };

    public LivroUseCaseTests()
    {
        _useCase = new LivroUseCase(
            _livroRepository.Object,
            _autorRepository.Object,
            _categoriaRepository.Object,
            _livroExterno.Object,
            new Mock<ILogger<LivroUseCase>>().Object);
    }

    [Fact]
    [Trait("UseCase", "Livro")]
    public async Task Adicionar_DeveRetornar201ComNomeDeAutorECategoria()
    {
        // Arrange
        _autorRepository.Setup(r => r.ObterUmAsync(1)).ReturnsAsync(new AutorEntity { Id = 1 });
        _categoriaRepository.Setup(r => r.ObterUmAsync(2)).ReturnsAsync(new CategoriaEntity { Id = 2 });
        _livroRepository.Setup(r => r.AdicionarAsync(It.IsAny<LivroEntity>()))
            .ReturnsAsync((LivroEntity e) => { e.Id = 10; return e; });
        _livroRepository.Setup(r => r.ObterUmAsync(10)).ReturnsAsync(new LivroEntity
        {
            Id = 10,
            Titulo = "Dom Casmurro",
            Autor = new AutorEntity { Nome = "Machado" },
            Categoria = new CategoriaEntity { Nome = "Romance" }
        });

        // Act
        var resultado = await _useCase.AdicionarAsync(NovoLivro());

        // Assert
        Assert.Equal(201, resultado.StatusCode);
        Assert.Equal("Machado", resultado.Value?.Autor);
        Assert.Equal("Romance", resultado.Value?.Categoria);
    }

    [Fact]
    [Trait("UseCase", "Livro")]
    public async Task Adicionar_AutorInexistente_DeveRetornar404ENaoSalvar()
    {
        // Arrange
        _autorRepository.Setup(r => r.ObterUmAsync(1)).ThrowsAsync(new NoContentException("Não foi encontrado autor com o id 1"));

        // Act
        var resultado = await _useCase.AdicionarAsync(NovoLivro());

        // Assert
        Assert.Equal(404, resultado.StatusCode);
        _livroRepository.Verify(r => r.AdicionarAsync(It.IsAny<LivroEntity>()), Times.Never);
    }

    [Fact]
    [Trait("UseCase", "Livro")]
    public async Task Adicionar_CategoriaInexistente_DeveRetornar404()
    {
        // Arrange
        _autorRepository.Setup(r => r.ObterUmAsync(1)).ReturnsAsync(new AutorEntity { Id = 1 });
        _categoriaRepository.Setup(r => r.ObterUmAsync(2)).ThrowsAsync(new NoContentException("Não foi encontrada categoria com o id 2"));

        // Act
        var resultado = await _useCase.AdicionarAsync(NovoLivro());

        // Assert
        Assert.Equal(404, resultado.StatusCode);
    }

    [Fact]
    [Trait("UseCase", "Livro")]
    public async Task ObterTodos_DeveRetornarPaginaComLinks()
    {
        // Arrange
        _livroRepository.Setup(r => r.ObterTodosAsync(0, 1))
            .ReturnsAsync((new List<LivroEntity> { new() { Id = 1, Titulo = "A" } }, 3));

        // Act
        var resultado = await _useCase.ObterTodosAsync(0, 1);

        // Assert
        Assert.Equal(200, resultado.StatusCode);
        Assert.Equal(3, resultado.Value!.TotalRegistros);
        Assert.Contains(resultado.Value.Links, l => l.Rel == "next");
        Assert.DoesNotContain(resultado.Value.Links, l => l.Rel == "prev");
    }

    [Fact]
    [Trait("UseCase", "Livro")]
    public async Task ObterPorIsbn_DeveRetornar200()
    {
        // Arrange
        _livroExterno.Setup(s => s.ObterPorIsbnAsync("123"))
            .ReturnsAsync(new EdicaoLivroModel { Isbn = "123", Titulo = "Externo" });

        // Act
        var resultado = await _useCase.ObterPorIsbnAsync("123");

        // Assert
        Assert.Equal(200, resultado.StatusCode);
        Assert.Equal("Externo", resultado.Value?.Titulo);
    }

    [Fact]
    [Trait("UseCase", "Livro")]
    public async Task ObterPorIsbn_NaoEncontrado_DeveRetornar404()
    {
        // Arrange
        _livroExterno.Setup(s => s.ObterPorIsbnAsync("000")).ThrowsAsync(new NoContentException("sem livro"));

        // Act
        var resultado = await _useCase.ObterPorIsbnAsync("000");

        // Assert
        Assert.Equal(404, resultado.StatusCode);
    }

    [Theory]
    [Trait("UseCase", "Livro")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ObterPorIsbn_Vazio_DeveRetornar400SemChamarServico(string isbn)
    {
        // Act
        var resultado = await _useCase.ObterPorIsbnAsync(isbn);

        // Assert
        Assert.Equal(400, resultado.StatusCode);
        _livroExterno.Verify(s => s.ObterPorIsbnAsync(It.IsAny<string>()), Times.Never);
    }
}
