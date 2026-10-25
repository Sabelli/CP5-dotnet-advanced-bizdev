using Biblioteca.API.Doc.Samples;
using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Biblioteca.API.Controllers;

[Authorize]
[Route("api/emprestimo")]
[ApiController]
public class EmprestimoController : ControllerBase
{
    private readonly IEmprestimoUseCase _emprestimoUseCase;

    public EmprestimoController(IEmprestimoUseCase emprestimoUseCase)
    {
        _emprestimoUseCase = emprestimoUseCase;
    }

    [HttpGet]
    [EnableRateLimiting("politica_listagem")]
    [SwaggerOperation(Summary = "Lista empréstimos paginados", Description = "Retorna os empréstimos com paginação por `Deslocamento` e `RegistroRetornado`. Requer autenticação.")]
    [SwaggerResponse(statusCode: 200, description: "Lista de empréstimos", type: typeof(PageResponseDto<GetEmprestimoDto>))]
    [SwaggerResponse(statusCode: 204, description: "Nenhum empréstimo encontrado")]
    [SwaggerResponse(statusCode: 400, description: "Erro ao buscar empréstimos", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(EmprestimoResponseListSample))]
    public async Task<IActionResult> Get(
        [SwaggerParameter("Quantidade de registros a pular")] int Deslocamento,
        [SwaggerParameter("Quantidade de registros retornados")] int RegistroRetornado)
    {
        var resultado = await _emprestimoUseCase.ObterTodosAsync(Deslocamento, RegistroRetornado);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obtém empréstimo por id")]
    [SwaggerResponse(statusCode: 200, description: "Empréstimo encontrado", type: typeof(GetEmprestimoDto))]
    [SwaggerResponse(statusCode: 400, description: "Erro ao obter empréstimo", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Empréstimo não encontrado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(EmprestimoResponseSample))]
    public async Task<IActionResult> Get([FromRoute, SwaggerParameter("Id do empréstimo")] int id)
    {
        var resultado = await _emprestimoUseCase.ObterUmAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Registra empréstimo", Description = "Empresta um livro a um usuário por `DiasEmprestimo` dias.")]
    [SwaggerRequestExample(typeof(PostEmprestimoDto), typeof(EmprestimoRequestSample))]
    [SwaggerResponse(statusCode: 201, description: "Empréstimo criado", type: typeof(GetEmprestimoDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Livro ou usuário não encontrado", type: typeof(string))]
    [SwaggerResponse(statusCode: 409, description: "Livro já emprestado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 201, typeof(EmprestimoResponseSample))]
    public async Task<IActionResult> Post([FromBody] PostEmprestimoDto dto)
    {
        var resultado = await _emprestimoUseCase.EmprestarAsync(dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPut("{id}/devolver")]
    [SwaggerOperation(Summary = "Devolve empréstimo", Description = "Registra a devolução do livro.")]
    [SwaggerResponse(statusCode: 200, description: "Devolução registrada", type: typeof(GetEmprestimoDto))]
    [SwaggerResponse(statusCode: 400, description: "Erro ao devolver", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Empréstimo não encontrado", type: typeof(string))]
    [SwaggerResponse(statusCode: 409, description: "Empréstimo já devolvido", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(EmprestimoResponseSample))]
    public async Task<IActionResult> Devolver([FromRoute, SwaggerParameter("Id do empréstimo")] int id)
    {
        var resultado = await _emprestimoUseCase.DevolverAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
