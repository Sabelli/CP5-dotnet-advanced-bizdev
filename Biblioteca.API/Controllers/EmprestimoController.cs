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

    /// <summary>
    /// Lista empréstimos paginados
    /// </summary>
    /// <returns>Lista de empréstimos</returns>
    [HttpGet]
    [EnableRateLimiting("politica_listagem")]
    [SwaggerOperation(
        Summary = "Lista empréstimos paginados",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Lista de empréstimos.
        * **Status 204 (No Content):** Nenhum empréstimo encontrado.
        * **Status 400 (Bad Request):** Erro ao buscar empréstimos.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        * Paginação via **Deslocamento** (registros a pular) e **RegistroRetornado** (tamanho da página).
        * A resposta inclui o **TotalRegistros** e links HATEOAS.
        """
    )]
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

    /// <summary>
    /// Obtém empréstimo por id
    /// </summary>
    /// <returns>Empréstimo encontrado</returns>
    [EnableRateLimiting("politica_listagem")]
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Obtém empréstimo por id",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Empréstimo encontrado.
        * **Status 400 (Bad Request):** Erro ao obter empréstimo.
        * **Status 404 (Not Found):** Empréstimo não encontrado.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        """
    )]
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

    /// <summary>
    /// Registra empréstimo
    /// </summary>
    /// <returns>Empréstimo criado</returns>
    [EnableRateLimiting("politica_escrita")]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Registra empréstimo",
        Description = """
        ## Informações do Retorno:
        * **Status 201 (Created):** Empréstimo criado.
        * **Status 400 (Bad Request):** Dados inválidos.
        * **Status 404 (Not Found):** Livro ou usuário não encontrado.
        * **Status 409 (Conflict):** Livro já emprestado.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        * Informe **LivroId**, **UsuarioId** e **DiasEmprestimo** (1 a 90, padrão 14).
        """
    )]
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

    /// <summary>
    /// Devolve empréstimo
    /// </summary>
    /// <returns>Devolução registrada</returns>
    [EnableRateLimiting("politica_escrita")]
    [HttpPut("{id}/devolver")]
    [SwaggerOperation(
        Summary = "Devolve empréstimo",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Devolução registrada.
        * **Status 400 (Bad Request):** Erro ao devolver.
        * **Status 404 (Not Found):** Empréstimo não encontrado.
        * **Status 409 (Conflict):** Empréstimo já devolvido.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        * Preenche a **DataDevolucao** do empréstimo.
        """
    )]
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
