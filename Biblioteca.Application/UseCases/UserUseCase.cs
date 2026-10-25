using System.Net;
using System.Net.Mail;
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
            if (!MailAddress.TryCreate(dto.Email, out _))
                throw new RequisicaoInvalidaException("E-mail inválido.");

            if (await EmailExisteAsync(dto.Email))
                throw new ConflitoException("E-mail já cadastrado.");

            var result = await _userRepository.AdicionarAsync(dto.ToUserEntity(SenhaHasher.Gerar(dto.Password)));
            return OperationResult<GetUserDto>.Success(result.ToUserDto(), (int)HttpStatusCode.Created);
        }
        catch (RequisicaoInvalidaException ex)
        {
            return OperationResult<GetUserDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
        }
        catch (ConflitoException ex)
        {
            _logger.LogWarning("Cadastro recusado: {0}", ex.Message);
            return OperationResult<GetUserDto>.Failure(ex.Message, (int)HttpStatusCode.Conflict);
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
                throw new NaoAutorizadoException("E-mail ou senha incorretos.");

            var auth = new GetAuthDto { Email = user.Email, Jwt = _tokenService.GerarToken(user.Email, user.Role) };
            return OperationResult<GetAuthDto>.Success(auth, (int)HttpStatusCode.OK);
        }
        catch (Exception ex) when (ex is NoContentException or NaoAutorizadoException)
        {
            return OperationResult<GetAuthDto>.Failure("E-mail ou senha incorretos.", (int)HttpStatusCode.Unauthorized);
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
