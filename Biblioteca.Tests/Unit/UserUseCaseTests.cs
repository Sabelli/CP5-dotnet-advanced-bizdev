using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Biblioteca.Application.Dtos;
using Biblioteca.Application.Services;
using Biblioteca.Application.UseCases;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Biblioteca.Tests.Unit;

public class UserUseCaseTests
{
    private readonly Mock<IUserRepository> _repository = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly UserUseCase _useCase;

    public UserUseCaseTests()
    {
        _useCase = new UserUseCase(_repository.Object, _tokenService.Object, new Mock<ILogger<UserUseCase>>().Object);
    }

    private static PostUserDto NovoUsuario() => new()
    {
        Nome = "Ana",
        Email = "ana@teste.com",
        Password = "Senha123",
        ConfirmPassword = "Senha123",
        Role = "leitor"
    };

    [Fact]
    [Trait("UseCase", "User")]
    public async Task Adicionar_DeveSalvarHashENaoDevolverSenha()
    {
        // Arrange
        UserEntity? salvo = null;
        _repository.Setup(r => r.ObterPorEmailAsync("ana@teste.com")).ThrowsAsync(new NoContentException("Usuário não encontrado"));
        _repository.Setup(r => r.AdicionarAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync((UserEntity e) => { salvo = e; e.Id = 1; return e; });

        // Act
        var resultado = await _useCase.AdicionarUserAsync(NovoUsuario());

        // Assert
        Assert.Equal(201, resultado.StatusCode);
        Assert.NotNull(salvo);
        Assert.NotEqual("Senha123", salvo!.PasswordHash);
        Assert.True(SenhaHasher.Verificar("Senha123", salvo.PasswordHash));
        Assert.Equal("leitor", resultado.Value?.Role);
    }

    [Fact]
    [Trait("UseCase", "User")]
    public async Task Adicionar_EmailDuplicado_DeveRetornar409()
    {
        // Arrange
        _repository.Setup(r => r.ObterPorEmailAsync("ana@teste.com")).ReturnsAsync(new UserEntity { Email = "ana@teste.com" });

        // Act
        var resultado = await _useCase.AdicionarUserAsync(NovoUsuario());

        // Assert
        Assert.Equal(409, resultado.StatusCode);
        _repository.Verify(r => r.AdicionarAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    [Trait("UseCase", "User")]
    public async Task Autentica_CredenciaisValidas_DeveRetornarJwt()
    {
        // Arrange
        _repository.Setup(r => r.ObterPorEmailAsync("ana@teste.com"))
            .ReturnsAsync(new UserEntity { Email = "ana@teste.com", Role = "admin", PasswordHash = SenhaHasher.Gerar("Senha123") });
        _tokenService.Setup(t => t.GerarToken("ana@teste.com", "admin")).Returns("jwt-fake");

        // Act
        var resultado = await _useCase.AutenticaUserAsync(new PostAuthDto { Email = "ana@teste.com", Senha = "Senha123" });

        // Assert
        Assert.Equal(200, resultado.StatusCode);
        Assert.Equal("jwt-fake", resultado.Value?.Jwt);
    }

    [Fact]
    [Trait("UseCase", "User")]
    public async Task Autentica_SenhaErrada_DeveRetornar404()
    {
        // Arrange
        _repository.Setup(r => r.ObterPorEmailAsync("ana@teste.com"))
            .ReturnsAsync(new UserEntity { Email = "ana@teste.com", PasswordHash = SenhaHasher.Gerar("Senha123") });

        // Act
        var resultado = await _useCase.AutenticaUserAsync(new PostAuthDto { Email = "ana@teste.com", Senha = "errada" });

        // Assert
        Assert.Equal(404, resultado.StatusCode);
        _tokenService.Verify(t => t.GerarToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait("UseCase", "User")]
    public async Task Autentica_EmailInexistente_DeveRetornar404()
    {
        // Arrange
        _repository.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>())).ThrowsAsync(new NoContentException("Usuário não encontrado"));

        // Act
        var resultado = await _useCase.AutenticaUserAsync(new PostAuthDto { Email = "x@x.com", Senha = "123456" });

        // Assert
        Assert.Equal(404, resultado.StatusCode);
    }
}

public class SenhaHasherTests
{
    [Fact]
    [Trait("Service", "SenhaHasher")]
    public void Gerar_MesmaSenha_DeveProduzirHashesDiferentesQueVerificam()
    {
        // Act
        var h1 = SenhaHasher.Gerar("Senha123");
        var h2 = SenhaHasher.Gerar("Senha123");

        // Assert
        Assert.NotEqual(h1, h2);
        Assert.True(SenhaHasher.Verificar("Senha123", h1));
        Assert.True(SenhaHasher.Verificar("Senha123", h2));
    }

    [Theory]
    [Trait("Service", "SenhaHasher")]
    [InlineData("senha123")]
    [InlineData("")]
    public void Verificar_SenhaIncorreta_DeveRetornarFalse(string tentativa)
    {
        // Arrange
        var hash = SenhaHasher.Gerar("Senha123");

        // Act & Assert
        Assert.False(SenhaHasher.Verificar(tentativa, hash));
    }

    [Fact]
    [Trait("Service", "SenhaHasher")]
    public void Verificar_FormatoInvalido_DeveRetornarFalse()
    {
        Assert.False(SenhaHasher.Verificar("x", "lixo"));
    }
}

public class TokenServiceTests
{
    [Fact]
    [Trait("Service", "TokenService")]
    public void GerarToken_DeveConterNomeERoleEExpirarEm8Horas()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Secretkey"] = "chave-de-teste-com-pelo-menos-32-bytes!!" })
            .Build();
        var service = new TokenService(config);

        // Act
        var jwt = service.GerarToken("ana@teste.com", "admin");
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

        // Assert
        Assert.Equal("ana@teste.com", token.Claims.First(c => c.Type == ClaimTypes.Name || c.Type == "unique_name").Value);
        Assert.Equal("admin", token.Claims.First(c => c.Type == ClaimTypes.Role || c.Type == "role").Value);
        Assert.InRange(token.ValidTo, DateTime.UtcNow.AddHours(7.9), DateTime.UtcNow.AddHours(8.1));
    }
}
