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
[Route("api/categoria")]
[ApiController]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaUseCase _categoriaUseCase;

    public CategoriaController(ICategoriaUseCase categoriaUseCase)
    {
        _categoriaUseCase = categoriaUseCase;
    }

    /// <summary>
    /// Lista categorias paginadas
    /// </summary>
    /// <returns>Lista de categorias</returns>
    [HttpGet]
    [EnableRateLimiting("politica_listagem")]
    [SwaggerOperation(
        Summary = "Lista categorias paginadas",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Lista de categorias.
        * **Status 204 (No Content):** Nenhuma categoria encontrada.
        * **Status 400 (Bad Request):** Erro ao buscar categorias.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        * Paginação via **Deslocamento** (registros a pular) e **RegistroRetornado** (tamanho da página).
        * A resposta inclui o **TotalRegistros** e links HATEOAS.
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Lista de categorias", type: typeof(PageResponseDto<GetCategoriaDto>))]
    [SwaggerResponse(statusCode: 204, description: "Nenhuma categoria encontrada")]
    [SwaggerResponse(statusCode: 400, description: "Erro ao buscar categorias", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(CategoriaResponseListSample))]
    public async Task<IActionResult> Get(
        [SwaggerParameter("Quantidade de registros a pular")] int Deslocamento,
        [SwaggerParameter("Quantidade de registros retornados")] int RegistroRetornado)
    {
        var resultado = await _categoriaUseCase.ObterTodosAsync(Deslocamento, RegistroRetornado);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Obtém categoria por id
    /// </summary>
    /// <returns>Categoria encontrada</returns>
    [EnableRateLimiting("politica_listagem")]
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Obtém categoria por id",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Categoria encontrada.
        * **Status 400 (Bad Request):** Erro ao obter categoria.
        * **Status 404 (Not Found):** Categoria não encontrada.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Categoria encontrada", type: typeof(GetCategoriaDto))]
    [SwaggerResponse(statusCode: 400, description: "Erro ao obter categoria", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Categoria não encontrada", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(CategoriaResponseSample))]
    public async Task<IActionResult> Get([FromRoute, SwaggerParameter("Id da categoria")] int id)
    {
        var resultado = await _categoriaUseCase.ObterUmAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Cadastra categoria
    /// </summary>
    /// <returns>Categoria criada</returns>
    [Authorize(Roles = "admin")]
    [EnableRateLimiting("politica_escrita")]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra categoria",
        Description = """
        ## Informações do Retorno:
        * **Status 201 (Created):** Categoria criada.
        * **Status 400 (Bad Request):** Dados inválidos.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        """
    )]
    [SwaggerRequestExample(typeof(PostCategoriaDto), typeof(CategoriaRequestSample))]
    [SwaggerResponse(statusCode: 201, description: "Categoria criada", type: typeof(GetCategoriaDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 201, typeof(CategoriaResponseSample))]
    public async Task<IActionResult> Post([FromBody] PostCategoriaDto dto)
    {
        var resultado = await _categoriaUseCase.AdicionarAsync(dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Edita categoria
    /// </summary>
    /// <returns>Categoria atualizada</returns>
    [Authorize(Roles = "admin")]
    [EnableRateLimiting("politica_escrita")]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Edita categoria",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Categoria atualizada.
        * **Status 400 (Bad Request):** Dados inválidos.
        * **Status 404 (Not Found):** Categoria não encontrada.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        """
    )]
    [SwaggerRequestExample(typeof(PostCategoriaDto), typeof(CategoriaRequestSample))]
    [SwaggerResponse(statusCode: 200, description: "Categoria atualizada", type: typeof(GetCategoriaDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Categoria não encontrada", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(CategoriaResponseSample))]
    public async Task<IActionResult> Put([FromRoute, SwaggerParameter("Id da categoria")] int id, [FromBody] PostCategoriaDto dto)
    {
        var resultado = await _categoriaUseCase.EditarAsync(id, dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Remove categoria
    /// </summary>
    /// <returns>Categoria removida</returns>
    [Authorize(Roles = "admin")]
    [EnableRateLimiting("politica_escrita")]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Remove categoria",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Categoria removida.
        * **Status 400 (Bad Request):** Erro ao remover categoria.
        * **Status 404 (Not Found):** Categoria não encontrada.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Categoria removida", type: typeof(GetCategoriaDto))]
    [SwaggerResponse(statusCode: 400, description: "Erro ao remover categoria", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Categoria não encontrada", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(CategoriaResponseSample))]
    public async Task<IActionResult> Delete([FromRoute, SwaggerParameter("Id da categoria")] int id)
    {
        var resultado = await _categoriaUseCase.DeletarAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
