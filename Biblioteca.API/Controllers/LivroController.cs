using Biblioteca.API.Doc.Samples;
using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Biblioteca.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Biblioteca.API.Controllers;

[Authorize]
[Route("api/livro")]
[ApiController]
public class LivroController : ControllerBase
{
    private readonly ILivroUseCase _livroUseCase;

    public LivroController(ILivroUseCase livroUseCase)
    {
        _livroUseCase = livroUseCase;
    }

    /// <summary>
    /// Lista livros paginados
    /// </summary>
    /// <returns>Lista de livros</returns>
    [HttpGet]
    [EnableRateLimiting("politica_listagem")]
    [SwaggerOperation(
        Summary = "Lista livros paginados",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Lista de livros.
        * **Status 204 (No Content):** Nenhum livro encontrado.
        * **Status 400 (Bad Request):** Erro ao buscar livros.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        * Paginação via **Deslocamento** (registros a pular) e **RegistroRetornado** (tamanho da página).
        * A resposta inclui o **TotalRegistros** e links HATEOAS.
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Lista de livros", type: typeof(PageResponseDto<GetLivroDto>))]
    [SwaggerResponse(statusCode: 204, description: "Nenhum livro encontrado")]
    [SwaggerResponse(statusCode: 400, description: "Erro ao buscar livros", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(LivroResponseListSample))]
    public async Task<IActionResult> Get(
        [SwaggerParameter("Quantidade de registros a pular")] int Deslocamento,
        [SwaggerParameter("Quantidade de registros retornados")] int RegistroRetornado)
    {
        var resultado = await _livroUseCase.ObterTodosAsync(Deslocamento, RegistroRetornado);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Consulta livro por ISBN
    /// </summary>
    /// <returns>Dados do livro</returns>
    [HttpGet("isbn/{isbn}")]
    [SwaggerOperation(
        Summary = "Consulta livro por ISBN",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Dados do livro.
        * **Status 400 (Bad Request):** ISBN inválido ou erro na consulta.
        * **Status 404 (Not Found):** ISBN não encontrado.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        * Os dados são buscados em serviço externo a partir do ISBN informado.
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Dados do livro", type: typeof(EdicaoLivroModel))]
    [SwaggerResponse(statusCode: 400, description: "ISBN inválido ou erro na consulta", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "ISBN não encontrado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(LivroIsbnResponseSample))]
    public async Task<IActionResult> GetPorIsbn([FromRoute, SwaggerParameter("ISBN do livro")] string isbn)
    {
        var resultado = await _livroUseCase.ObterPorIsbnAsync(isbn);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Obtém livro por id
    /// </summary>
    /// <returns>Livro encontrado</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Obtém livro por id",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Livro encontrado.
        * **Status 400 (Bad Request):** Erro ao obter livro.
        * **Status 404 (Not Found):** Livro não encontrado.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Livro encontrado", type: typeof(GetLivroDto))]
    [SwaggerResponse(statusCode: 400, description: "Erro ao obter livro", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Livro não encontrado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(LivroResponseSample))]
    public async Task<IActionResult> Get([FromRoute, SwaggerParameter("Id do livro")] int id)
    {
        var resultado = await _livroUseCase.ObterUmAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Cadastra livro
    /// </summary>
    /// <returns>Livro criado</returns>
    [Authorize(Roles = "admin")]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra livro",
        Description = """
        ## Informações do Retorno:
        * **Status 201 (Created):** Livro criado.
        * **Status 400 (Bad Request):** Dados inválidos.
        * **Status 404 (Not Found):** Autor ou categoria não encontrado.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        * **AutorId** e **CategoriaId** devem referenciar registros existentes.
        """
    )]
    [SwaggerRequestExample(typeof(PostLivroDto), typeof(LivroRequestSample))]
    [SwaggerResponse(statusCode: 201, description: "Livro criado", type: typeof(GetLivroDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Autor ou categoria não encontrado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 201, typeof(LivroResponseSample))]
    public async Task<IActionResult> Post([FromBody] PostLivroDto dto)
    {
        var resultado = await _livroUseCase.AdicionarAsync(dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Edita livro
    /// </summary>
    /// <returns>Livro atualizado</returns>
    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Edita livro",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Livro atualizado.
        * **Status 400 (Bad Request):** Dados inválidos.
        * **Status 404 (Not Found):** Livro não encontrado.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        """
    )]
    [SwaggerRequestExample(typeof(PostLivroDto), typeof(LivroRequestSample))]
    [SwaggerResponse(statusCode: 200, description: "Livro atualizado", type: typeof(GetLivroDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Livro não encontrado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(LivroResponseSample))]
    public async Task<IActionResult> Put([FromRoute, SwaggerParameter("Id do livro")] int id, [FromBody] PostLivroDto dto)
    {
        var resultado = await _livroUseCase.EditarAsync(id, dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    /// <summary>
    /// Remove livro
    /// </summary>
    /// <returns>Livro removido</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Remove livro",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Livro removido.
        * **Status 400 (Bad Request):** Erro ao remover livro.
        * **Status 404 (Not Found):** Livro não encontrado.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Livro removido", type: typeof(GetLivroDto))]
    [SwaggerResponse(statusCode: 400, description: "Erro ao remover livro", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Livro não encontrado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(LivroResponseSample))]
    public async Task<IActionResult> Delete([FromRoute, SwaggerParameter("Id do livro")] int id)
    {
        var resultado = await _livroUseCase.DeletarAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
