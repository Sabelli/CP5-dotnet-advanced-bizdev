using Biblioteca.Application.Dtos;
using Biblioteca.Application.UseCases;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Biblioteca.Tests.Unit;

public class AutorUseCaseTests
{
    private readonly Mock<IAutorRepository> _repository = new();
    private readonly AutorUseCase _useCase;

    public AutorUseCaseTests()
    {
        _useCase = new AutorUseCase(_repository.Object, new Mock<ILogger<AutorUseCase>>().Object);
    }

    [Fact]
    [Trait("UseCase", "Autor")]
    public async Task ObterUm_DeveRetornar200ComAutor()
    {
        // Arrange
        _repository.Setup(r => r.ObterUmAsync(1)).ReturnsAsync(new AutorEntity { Id = 1, Nome = "Machado" });

        // Act
        var resultado = await _useCase.ObterUmAsync(1);

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.Equal(200, resultado.StatusCode);
        Assert.Equal("Machado", resultado.Value?.Nome);
    }

    [Fact]
    [Trait("UseCase", "Autor")]
    public async Task ObterUm_Inexistente_DeveRetornar404()
    {
        // Arrange
        _repository.Setup(r => r.ObterUmAsync(99)).ThrowsAsync(new NoContentException("Não foi encontrado autor com o id 99"));

        // Act
        var resultado = await _useCase.ObterUmAsync(99);

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal(404, resultado.StatusCode);
        Assert.Equal("Não foi encontrado autor com o id 99", resultado.Error);
    }

    [Fact]
    [Trait("UseCase", "Autor")]
    public async Task ObterTodos_SemRegistros_DeveRetornar204()
    {
        // Arrange
        _repository.Setup(r => r.ObterTodosAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new NoContentException("Não foram encontrados autores"));

        // Act
        var resultado = await _useCase.ObterTodosAsync(0, 30);

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal(204, resultado.StatusCode);
    }

    [Fact]
    [Trait("UseCase", "Autor")]
    public async Task ObterTodos_ParametrosInvalidos_DeveSanitizarNaPagina()
    {
        // Arrange
        _repository.Setup(r => r.ObterTodosAsync(0, 30))
            .ReturnsAsync((new List<AutorEntity> { new() { Id = 1, Nome = "Machado" } }, 1));

        // Act
        var resultado = await _useCase.ObterTodosAsync(-5, 0);

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.Equal(0, resultado.Value!.Deslocamento);
        Assert.Equal(30, resultado.Value.RegistroRetornado);
        Assert.Equal(1, resultado.Value.TotalRegistros);
    }

    [Fact]
    [Trait("UseCase", "Autor")]
    public async Task ObterTodos_ErroInesperado_DeveRetornar400()
    {
        // Arrange
        _repository.Setup(r => r.ObterTodosAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new Exception("banco fora"));

        // Act
        var resultado = await _useCase.ObterTodosAsync(0, 30);

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal(400, resultado.StatusCode);
    }

    [Fact]
    [Trait("UseCase", "Autor")]
    public async Task Adicionar_DeveRetornar201()
    {
        // Arrange
        _repository.Setup(r => r.AdicionarAsync(It.IsAny<AutorEntity>()))
            .ReturnsAsync((AutorEntity e) => { e.Id = 5; return e; });

        // Act
        var resultado = await _useCase.AdicionarAsync(new PostAutorDto { Nome = "Clarice", Nacionalidade = "BR" });

        // Assert
        Assert.Equal(201, resultado.StatusCode);
        Assert.Equal(5, resultado.Value?.Id);
        Assert.Equal("Clarice", resultado.Value?.Nome);
    }

    [Fact]
    [Trait("UseCase", "Autor")]
    public async Task Deletar_Inexistente_DeveRetornar404()
    {
        // Arrange
        _repository.Setup(r => r.DeletarAsync(7)).ThrowsAsync(new NoContentException("não existe"));

        // Act
        var resultado = await _useCase.DeletarAsync(7);

        // Assert
        Assert.Equal(404, resultado.StatusCode);
    }
}
