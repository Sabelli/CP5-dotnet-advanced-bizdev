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
[Route("api/autor")]
[ApiController]
public class AutorController : ControllerBase
{
    private readonly IAutorUseCase _autorUseCase;

    public AutorController(IAutorUseCase autorUseCase)
    {
        _autorUseCase = autorUseCase;
    }

    /// <summary>
    /// Lista autores paginados
    /// </summary>
    /// <returns>Lista de autores</returns>
    [HttpGet]
    [EnableRateLimiting("politica_listagem")]
    [SwaggerOperation(
        Summary = "Lista autores paginados",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Lista de autores.
        * **Status 204 (No Content):** Nenhum autor encontrado.
        * **Status 400 (Bad Request):** Erro ao buscar autores.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        * Paginação via **Deslocamento** (registros a pular) e **RegistroRetornado** (tamanho da página).
        * A resposta inclui o **TotalRegistros** e links HATEOAS.
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Lista de autores", type: typeof(PageResponseDto<GetAutorDto>))]
    [SwaggerResponse(statusCode: 204, description: "Nenhum autor encontrado")]
    [SwaggerResponse(statusCode: 400, description: "Erro ao buscar autores", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(AutorResponseListSample))]
    public async Task<IActionResult> Get(
        [SwaggerParameter("Quantidade de registros a pular")] int Deslocamento,
        [SwaggerParameter("Quantidade de registros retornados")] int RegistroRetornado)
    {
        var resultado = await _autorUseCase.ObterTodosAsync(Deslocamento, RegistroRetornado);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Obtém autor por id
    /// </summary>
    /// <returns>Autor encontrado</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Obtém autor por id",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Autor encontrado.
        * **Status 400 (Bad Request):** Erro ao obter autor.
        * **Status 404 (Not Found):** Autor não encontrado.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Autor encontrado", type: typeof(GetAutorDto))]
    [SwaggerResponse(statusCode: 400, description: "Erro ao obter autor", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Autor não encontrado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(AutorResponseSample))]
    public async Task<IActionResult> Get([FromRoute, SwaggerParameter("Id do autor")] int id)
    {
        var resultado = await _autorUseCase.ObterUmAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Cadastra autor
    /// </summary>
    /// <returns>Autor criado</returns>
    [Authorize(Roles = "admin")]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra autor",
        Description = """
        ## Informações do Retorno:
        * **Status 201 (Created):** Autor criado.
        * **Status 400 (Bad Request):** Dados inválidos.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        """
    )]
    [SwaggerRequestExample(typeof(PostAutorDto), typeof(AutorRequestSample))]
    [SwaggerResponse(statusCode: 201, description: "Autor criado", type: typeof(GetAutorDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 201, typeof(AutorResponseSample))]
    public async Task<IActionResult> Post([FromBody] PostAutorDto dto)
    {
        var resultado = await _autorUseCase.AdicionarAsync(dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Edita autor
    /// </summary>
    /// <returns>Autor atualizado</returns>
    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Edita autor",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Autor atualizado.
        * **Status 400 (Bad Request):** Dados inválidos.
        * **Status 404 (Not Found):** Autor não encontrado.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        """
    )]
    [SwaggerRequestExample(typeof(PostAutorDto), typeof(AutorRequestSample))]
    [SwaggerResponse(statusCode: 200, description: "Autor atualizado", type: typeof(GetAutorDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Autor não encontrado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(AutorResponseSample))]
    public async Task<IActionResult> Put([FromRoute, SwaggerParameter("Id do autor")] int id, [FromBody] PostAutorDto dto)
    {
        var resultado = await _autorUseCase.EditarAsync(id, dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Remove autor
    /// </summary>
    /// <returns>Autor removido</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Remove autor",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Autor removido.
        * **Status 400 (Bad Request):** Erro ao remover autor.
        * **Status 404 (Not Found):** Autor não encontrado.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Autor removido", type: typeof(GetAutorDto))]
    [SwaggerResponse(statusCode: 400, description: "Erro ao remover autor", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Autor não encontrado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(AutorResponseSample))]
    public async Task<IActionResult> Delete([FromRoute, SwaggerParameter("Id do autor")] int id)
    {
        var resultado = await _autorUseCase.DeletarAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
