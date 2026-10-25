using System.Net;
using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Biblioteca.Application.Mappers;
using Biblioteca.Application.Services;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Biblioteca.Application.UseCases;

public class UserUseCase : IUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<UserUseCase> _logger;

    public UserUseCase(IUserRepository userRepository, ITokenService tokenService, ILogger<UserUseCase> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<OperationResult<GetUserDto>> AdicionarUserAsync(PostUserDto dto)
    {
        try
        {
            if (await EmailExisteAsync(dto.Email))
                return OperationResult<GetUserDto>.Failure($"Já existe usuário com o e-mail {dto.Email}", (int)HttpStatusCode.Conflict);

            var result = await _userRepository.AdicionarAsync(dto.ToUserEntity(SenhaHasher.Gerar(dto.Password)));
            return OperationResult<GetUserDto>.Success(result.ToUserDto(), (int)HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetUserDto>.Failure("Ocorreu um erro ao cadastrar o usuário", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetAuthDto>> AutenticaUserAsync(PostAuthDto dto)
    {
        try
        {
            var user = await _userRepository.ObterPorEmailAsync(dto.Email);

            // Mesma resposta para e-mail inexistente e senha errada, para não revelar quais e-mails existem.
            if (!SenhaHasher.Verificar(dto.Senha, user.PasswordHash))
                throw new NoContentException("Usuário ou senha inválidos");

            var auth = new GetAuthDto { Email = user.Email, Jwt = _tokenService.GerarToken(user.Email, user.Role) };
            return OperationResult<GetAuthDto>.Success(auth, (int)HttpStatusCode.OK);
        }
        catch (NoContentException)
        {
            return OperationResult<GetAuthDto>.Failure("Usuário ou senha inválidos", (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetAuthDto>.Failure("Ocorreu um erro ao autenticar o usuário", (int)HttpStatusCode.BadRequest);
        }
    }

    private async Task<bool> EmailExisteAsync(string email)
    {
        try
        {
            await _userRepository.ObterPorEmailAsync(email);
            return true;
        }
        catch (NoContentException)
        {
            return false;
        }
    }
}
