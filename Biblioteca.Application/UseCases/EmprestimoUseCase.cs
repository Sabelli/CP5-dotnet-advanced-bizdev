using System.Net;
using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Biblioteca.Application.Mappers;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Biblioteca.Application.UseCases;

public class EmprestimoUseCase : IEmprestimoUseCase
{
    private readonly IEmprestimoRepository _repository;
    private readonly ILivroRepository _livroRepository;
    private readonly ILogger<EmprestimoUseCase> _logger;

    public EmprestimoUseCase(IEmprestimoRepository repository,
        ILivroRepository livroRepository,
        ILogger<EmprestimoUseCase> logger)
    {
        _repository = repository;
        _livroRepository = livroRepository;
        _logger = logger;
    }

    public async Task<OperationResult<PageResponseDto<GetEmprestimoDto>>> ObterTodosAsync(int Deslocamento, int RegistroRetornado)
    {
        try
        {
            if (Deslocamento < 0) Deslocamento = 0;
            if (RegistroRetornado <= 0) RegistroRetornado = 30;

            _logger.LogInformation("Iniciando o ObterTodosAsync de empréstimos na camada de application");
            var result = await _repository.ObterTodosAsync(Deslocamento, RegistroRetornado);

            var page = new PageResponseDto<GetEmprestimoDto>
            {
                Items = result.data.Select(x => x.ToEmprestimoDto()),
                Deslocamento = Deslocamento,
                RegistroRetornado = RegistroRetornado,
                TotalRegistros = result.rowsCount
            };

            return OperationResult<PageResponseDto<GetEmprestimoDto>>.Success(page, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Não retornou empréstimos para D: {0} R: {1}", Deslocamento, RegistroRetornado);
            return OperationResult<PageResponseDto<GetEmprestimoDto>>.Failure(ex.Message, (int)HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<PageResponseDto<GetEmprestimoDto>>.Failure("Ocorreu um erro ao buscar todos os empréstimos", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetEmprestimoDto>> ObterUmAsync(int Id)
    {
        try
        {
            _logger.LogInformation("Iniciando o ObterUmAsync de empréstimo na camada de application");
            var result = await _repository.ObterUmAsync(Id);
            return OperationResult<GetEmprestimoDto>.Success(result.ToEmprestimoDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Não retornou empréstimo para ID: {0}", Id);
            return OperationResult<GetEmprestimoDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetEmprestimoDto>.Failure("Ocorreu um erro ao obter o empréstimo", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetEmprestimoDto>> EmprestarAsync(PostEmprestimoDto dto)
    {
        try
        {
            await _livroRepository.ObterUmAsync(dto.LivroId);

            if (await _repository.LivroEmprestadoAsync(dto.LivroId))
                return OperationResult<GetEmprestimoDto>.Failure($"O livro {dto.LivroId} já está emprestado", (int)HttpStatusCode.Conflict);

            var added = await _repository.AdicionarAsync(dto.ToEmprestimoEntity());
            var result = await _repository.ObterUmAsync(added.Id);
            return OperationResult<GetEmprestimoDto>.Success(result.ToEmprestimoDto(), (int)HttpStatusCode.Created);
        }
        catch (NoContentException ex)
        {
            return OperationResult<GetEmprestimoDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetEmprestimoDto>.Failure("Ocorreu um erro ao salvar o empréstimo", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetEmprestimoDto>> DevolverAsync(int Id)
    {
        try
        {
            var atual = await _repository.ObterUmAsync(Id);

            if (atual.DataDevolucao != null)
                return OperationResult<GetEmprestimoDto>.Failure($"O empréstimo {Id} já foi devolvido", (int)HttpStatusCode.Conflict);

            await _repository.DevolverAsync(Id);
            var result = await _repository.ObterUmAsync(Id);
            return OperationResult<GetEmprestimoDto>.Success(result.ToEmprestimoDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<GetEmprestimoDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetEmprestimoDto>.Failure("Ocorreu um erro ao devolver o empréstimo", (int)HttpStatusCode.BadRequest);
        }
    }
}
