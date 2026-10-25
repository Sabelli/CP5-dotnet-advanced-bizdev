using System.Net;
using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Biblioteca.Application.Mappers;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Biblioteca.Application.UseCases;

public class AutorUseCase : IAutorUseCase
{
    private readonly IAutorRepository _repository;
    private readonly ILogger<AutorUseCase> _logger;

    public AutorUseCase(IAutorRepository repository, ILogger<AutorUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<OperationResult<PageResponseDto<GetAutorDto>>> ObterTodosAsync(int Deslocamento, int RegistroRetornado)
    {
        try
        {
            if (Deslocamento < 0) Deslocamento = 0;
            if (RegistroRetornado <= 0) RegistroRetornado = 30;

            _logger.LogInformation("Iniciando o ObterTodosAsync de autores na camada de application");
            var result = await _repository.ObterTodosAsync(Deslocamento, RegistroRetornado);

            var page = new PageResponseDto<GetAutorDto>
            {
                Items = result.data.Select(x => x.ToAutorDto()),
                Deslocamento = Deslocamento,
                RegistroRetornado = RegistroRetornado,
                TotalRegistros = result.rowsCount,
                Links = PageLinks.Build("/api/autor", "autor", Deslocamento, RegistroRetornado, result.rowsCount)
            };

            return OperationResult<PageResponseDto<GetAutorDto>>.Success(page, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Não retornou autores para D: {0} R: {1}", Deslocamento, RegistroRetornado);
            return OperationResult<PageResponseDto<GetAutorDto>>.Failure(ex.Message, (int)HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<PageResponseDto<GetAutorDto>>.Failure("Ocorreu um erro ao buscar todos os autores", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetAutorDto>> ObterUmAsync(int Id)
    {
        try
        {
            _logger.LogInformation("Iniciando o ObterUmAsync de autor na camada de application");
            var result = await _repository.ObterUmAsync(Id);
            return OperationResult<GetAutorDto>.Success(result.ToAutorDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Não retornou autor para ID: {0}", Id);
            return OperationResult<GetAutorDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetAutorDto>.Failure("Ocorreu um erro ao obter o autor", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetAutorDto>> AdicionarAsync(PostAutorDto dto)
    {
        try
        {
            var result = await _repository.AdicionarAsync(dto.ToAutorEntity());
            return OperationResult<GetAutorDto>.Success(result.ToAutorDto(), (int)HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetAutorDto>.Failure("Ocorreu um erro ao salvar o autor", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetAutorDto>> EditarAsync(int Id, PostAutorDto dto)
    {
        try
        {
            var result = await _repository.EditarAsync(Id, dto.ToAutorEntity());
            return OperationResult<GetAutorDto>.Success(result.ToAutorDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<GetAutorDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetAutorDto>.Failure("Ocorreu um erro ao editar o autor", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetAutorDto>> DeletarAsync(int Id)
    {
        try
        {
            var result = await _repository.DeletarAsync(Id);
            return OperationResult<GetAutorDto>.Success(result.ToAutorDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<GetAutorDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetAutorDto>.Failure("Ocorreu um erro ao deletar o autor", (int)HttpStatusCode.BadRequest);
        }
    }
}
