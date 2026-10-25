using System.Net;
using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Biblioteca.Application.Mappers;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Biblioteca.Application.UseCases;

public class LivroUseCase : ILivroUseCase
{
    private readonly ILivroRepository _repository;
    private readonly IAutorRepository _autorRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly ILogger<LivroUseCase> _logger;

    public LivroUseCase(ILivroRepository repository,
        IAutorRepository autorRepository,
        ICategoriaRepository categoriaRepository,
        ILogger<LivroUseCase> logger)
    {
        _repository = repository;
        _autorRepository = autorRepository;
        _categoriaRepository = categoriaRepository;
        _logger = logger;
    }

    public async Task<OperationResult<PageResponseDto<GetLivroDto>>> ObterTodosAsync(int Deslocamento, int RegistroRetornado)
    {
        try
        {
            if (Deslocamento < 0) Deslocamento = 0;
            if (RegistroRetornado <= 0) RegistroRetornado = 30;

            _logger.LogInformation("Iniciando o ObterTodosAsync de livros na camada de application");
            var result = await _repository.ObterTodosAsync(Deslocamento, RegistroRetornado);

            var page = new PageResponseDto<GetLivroDto>
            {
                Items = result.data.Select(x => x.ToLivroDto()),
                Deslocamento = Deslocamento,
                RegistroRetornado = RegistroRetornado,
                TotalRegistros = result.rowsCount,
                Links = PageLinks.Build("/api/livro", "livro", Deslocamento, RegistroRetornado, result.rowsCount)
            };

            return OperationResult<PageResponseDto<GetLivroDto>>.Success(page, (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Não retornou livros para D: {0} R: {1}", Deslocamento, RegistroRetornado);
            return OperationResult<PageResponseDto<GetLivroDto>>.Failure(ex.Message, (int)HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<PageResponseDto<GetLivroDto>>.Failure("Ocorreu um erro ao buscar todos os livros", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetLivroDto>> ObterUmAsync(int Id)
    {
        try
        {
            _logger.LogInformation("Iniciando o ObterUmAsync de livro na camada de application");
            var result = await _repository.ObterUmAsync(Id);
            return OperationResult<GetLivroDto>.Success(result.ToLivroDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            _logger.LogWarning("Não retornou livro para ID: {0}", Id);
            return OperationResult<GetLivroDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetLivroDto>.Failure("Ocorreu um erro ao obter o livro", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetLivroDto>> AdicionarAsync(PostLivroDto dto)
    {
        try
        {
            await _autorRepository.ObterUmAsync(dto.AutorId);
            await _categoriaRepository.ObterUmAsync(dto.CategoriaId);

            var added = await _repository.AdicionarAsync(dto.ToLivroEntity());
            var result = await _repository.ObterUmAsync(added.Id);
            return OperationResult<GetLivroDto>.Success(result.ToLivroDto(), (int)HttpStatusCode.Created);
        }
        catch (NoContentException ex)
        {
            return OperationResult<GetLivroDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetLivroDto>.Failure("Ocorreu um erro ao salvar o livro", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetLivroDto>> EditarAsync(int Id, PostLivroDto dto)
    {
        try
        {
            await _autorRepository.ObterUmAsync(dto.AutorId);
            await _categoriaRepository.ObterUmAsync(dto.CategoriaId);

            await _repository.EditarAsync(Id, dto.ToLivroEntity());
            var result = await _repository.ObterUmAsync(Id);
            return OperationResult<GetLivroDto>.Success(result.ToLivroDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<GetLivroDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetLivroDto>.Failure("Ocorreu um erro ao editar o livro", (int)HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<GetLivroDto>> DeletarAsync(int Id)
    {
        try
        {
            var result = await _repository.DeletarAsync(Id);
            return OperationResult<GetLivroDto>.Success(result.ToLivroDto(), (int)HttpStatusCode.OK);
        }
        catch (NoContentException ex)
        {
            return OperationResult<GetLivroDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("[ERROR] - {0}", ex.Message);
            return OperationResult<GetLivroDto>.Failure("Ocorreu um erro ao deletar o livro", (int)HttpStatusCode.BadRequest);
        }
    }
}
