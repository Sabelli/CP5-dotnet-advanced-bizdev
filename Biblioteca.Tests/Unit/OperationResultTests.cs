using Biblioteca.Domain.Entities;

namespace Biblioteca.Tests.Unit;

public class OperationResultTests
{
    [Theory]
    [Trait("Domain", "OperationResult")]
    [InlineData(200)]
    [InlineData(201)]
    public void Success_DeveMarcarSucessoComValorEStatus(int statusCode)
    {
        // Arrange & Act
        var resultado = OperationResult<string>.Success("ok", statusCode);

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.Equal("ok", resultado.Value);
        Assert.Equal(statusCode, resultado.StatusCode);
        Assert.Null(resultado.Error);
    }

    [Theory]
    [Trait("Domain", "OperationResult")]
    [InlineData("não encontrado", 404)]
    [InlineData("conflito", 409)]
    [InlineData("erro", 400)]
    public void Failure_DeveMarcarFalhaComMensagemEStatus(string erro, int statusCode)
    {
        // Arrange & Act
        var resultado = OperationResult<string>.Failure(erro, statusCode);

        // Assert
        Assert.False(resultado.IsSuccess);
        Assert.Equal(erro, resultado.Error);
        Assert.Equal(statusCode, resultado.StatusCode);
        Assert.Null(resultado.Value);
    }

    [Fact]
    [Trait("Domain", "OperationResult")]
    public void Failure_SemStatus_DeveUsar400()
    {
        // Arrange & Act
        var resultado = OperationResult<string>.Failure("erro");

        // Assert
        Assert.Equal(400, resultado.StatusCode);
    }
}
