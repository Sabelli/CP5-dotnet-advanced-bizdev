using System.Net;
using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Biblioteca.Application.Mappers;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Biblioteca.Application.UseCases;

public class CategoriaUseCase : ICategoriaUseCase
{
    private readonly ICategoriaRepository _repository;
    private readonly ILogger<CategoriaUseCase> _logger;

    public CategoriaUseCase(ICategoriaRepository repository, ILogger<CategoriaUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<OperationResult<PageResponseDto<GetCategoriaDto>>> ObterTodosAsync(int Deslocamento, int RegistroRetornado)
    {
        try
        {
            if (Deslocamento < 0) Deslocamento = 0;
            if (RegistroRetornado <= 0) RegistroRetornado = 30;

            _logger.LogInformation("Iniciando o ObterTodosAsync de categorias na camada de application");
            var result = await _repository.ObterTodosAsync(Deslocamento, RegistroRetornado);

            var page = new PageResponseDto<GetCategoriaDto>
            {
                Items = result.data.Select(x => x.ToCategoriaDto()),
                Deslocamento = Deslocamento,
                RegistroRetornado = RegistroRetornado,
                TotalRegistros = result.rowsCount
            };

            return OperationResult<PageResponseDto<GetCategoriaDto>>.Success(page, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Não retornou categorias para D: {0} R: {1}", Deslocamento, RegistroRetornado);
            return OperationResult<PageResponseDto<GetCategoriaDto>>.Failure(ex.Message, (int)HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<PageResponseDto<GetCategoriaDto>>.Failure("Ocorreu um erro ao buscar todas as categorias", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetCategoriaDto>> ObterUmAsync(int Id)
    {
        try
        {
            _logger.LogInformation("Iniciando o ObterUmAsync de categoria na camada de application");
            var result = await _repository.ObterUmAsync(Id);
            return OperationResult<GetCategoriaDto>.Success(result.ToCategoriaDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Não retornou categoria para ID: {0}", Id);
            return OperationResult<GetCategoriaDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetCategoriaDto>.Failure("Ocorreu um erro ao obter a categoria", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetCategoriaDto>> AdicionarAsync(PostCategoriaDto dto)
    {
        try
        {
            var result = await _repository.AdicionarAsync(dto.ToCategoriaEntity());
            return OperationResult<GetCategoriaDto>.Success(result.ToCategoriaDto(), (int)HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetCategoriaDto>.Failure("Ocorreu um erro ao salvar a categoria", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetCategoriaDto>> EditarAsync(int Id, PostCategoriaDto dto)
    {
        try
        {
            var result = await _repository.EditarAsync(Id, dto.ToCategoriaEntity());
            return OperationResult<GetCategoriaDto>.Success(result.ToCategoriaDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<GetCategoriaDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetCategoriaDto>.Failure("Ocorreu um erro ao editar a categoria", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetCategoriaDto>> DeletarAsync(int Id)
    {
        try
        {
            var result = await _repository.DeletarAsync(Id);
            return OperationResult<GetCategoriaDto>.Success(result.ToCategoriaDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<GetCategoriaDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetCategoriaDto>.Failure("Ocorreu um erro ao deletar a categoria", (int)HttpStatusCode.BadRequest);
        }
    }
}
