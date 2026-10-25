using Biblioteca.Application.Dtos;
using Biblioteca.Application.UseCases;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Biblioteca.Tests.Unit;

public class EmprestimoUseCaseTests
{
    private readonly Mock<IEmprestimoRepository> _repository = new();
    private readonly Mock<ILivroRepository> _livroRepository = new();
    private readonly EmprestimoUseCase _useCase;

    public EmprestimoUseCaseTests()
    {
        _useCase = new EmprestimoUseCase(_repository.Object, _livroRepository.Object, new Mock<ILogger<EmprestimoUseCase>>().Object);
    }

    [Fact]
    [Trait("UseCase", "Emprestimo")]
    public async Task Emprestar_LivroDisponivel_DeveRetornar201()
    {
        // Arrange
        _livroRepository.Setup(r => r.ObterUmAsync(1)).ReturnsAsync(new LivroEntity { Id = 1 });
        _repository.Setup(r => r.LivroEmprestadoAsync(1)).ReturnsAsync(false);
        _repository.Setup(r => r.AdicionarAsync(It.IsAny<EmprestimoEntity>()))
            .ReturnsAsync((EmprestimoEntity e) => { e.Id = 7; return e; });
        _repository.Setup(r => r.ObterUmAsync(7)).ReturnsAsync(new EmprestimoEntity { Id = 7, LivroId = 1, UsuarioId = 2 });

        // Act
        var resultado = await _useCase.EmprestarAsync(new PostEmprestimoDto { LivroId = 1, UsuarioId = 2, DiasEmprestimo = 7 });

        // Assert
        Assert.Equal(201, resultado.StatusCode);
        Assert.Equal(7, resultado.Value?.Id);
        Assert.Contains(resultado.Value!.Links, l => l.Rel == "devolver_emprestimo");
    }

    [Fact]
    [Trait("UseCase", "Emprestimo")]
    public async Task Emprestar_LivroJaEmprestado_DeveRetornar409()
    {
        // Arrange
        _livroRepository.Setup(r => r.ObterUmAsync(1)).ReturnsAsync(new LivroEntity { Id = 1 });
        _repository.Setup(r => r.LivroEmprestadoAsync(1)).ReturnsAsync(true);

        // Act
        var resultado = await _useCase.EmprestarAsync(new PostEmprestimoDto { LivroId = 1, UsuarioId = 2 });

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal(409, resultado.StatusCode);
        _repository.Verify(r => r.AdicionarAsync(It.IsAny<EmprestimoEntity>()), Times.Never);
    }

    [Fact]
    [Trait("UseCase", "Emprestimo")]
    public async Task Emprestar_LivroInexistente_DeveRetornar404()
    {
        // Arrange
        _livroRepository.Setup(r => r.ObterUmAsync(9)).ThrowsAsync(new NoContentException("Não foi encontrado livro com o id 9"));

        // Act
        var resultado = await _useCase.EmprestarAsync(new PostEmprestimoDto { LivroId = 9, UsuarioId = 2 });

        // Assert
        Assert.Equal(404, resultado.StatusCode);
    }

    [Fact]
    [Trait("UseCase", "Emprestimo")]
    public async Task Devolver_EmprestimoAberto_DeveRetornar200()
    {
        // Arrange
        var aberto = new EmprestimoEntity { Id = 3, DataDevolucao = null };
        var devolvido = new EmprestimoEntity { Id = 3, DataDevolucao = DateTime.UtcNow };
        _repository.SetupSequence(r => r.ObterUmAsync(3)).ReturnsAsync(aberto).ReturnsAsync(devolvido);
        _repository.Setup(r => r.DevolverAsync(3)).ReturnsAsync(devolvido);

        // Act
        var resultado = await _useCase.DevolverAsync(3);

        // Assert
        Assert.Equal(200, resultado.StatusCode);
        Assert.NotNull(resultado.Value?.DataDevolucao);
        Assert.DoesNotContain(resultado.Value!.Links, l => l.Rel == "devolver_emprestimo");
    }

    [Fact]
    [Trait("UseCase", "Emprestimo")]
    public async Task Devolver_JaDevolvido_DeveRetornar409()
    {
        // Arrange
        _repository.Setup(r => r.ObterUmAsync(3)).ReturnsAsync(new EmprestimoEntity { Id = 3, DataDevolucao = DateTime.UtcNow });

        // Act
        var resultado = await _useCase.DevolverAsync(3);

        // Assert
        Assert.Equal(409, resultado.StatusCode);
        _repository.Verify(r => r.DevolverAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    [Trait("UseCase", "Emprestimo")]
    public async Task Devolver_Inexistente_DeveRetornar404()
    {
        // Arrange
        _repository.Setup(r => r.ObterUmAsync(99)).ThrowsAsync(new NoContentException("não existe"));

        // Act
        var resultado = await _useCase.DevolverAsync(99);

        // Assert
        Assert.Equal(404, resultado.StatusCode);
    }
}
