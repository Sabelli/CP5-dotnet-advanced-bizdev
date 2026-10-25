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
    [EnableRateLimiting("politica_escrita")]
    [HttpGet("isbn/{isbn}")]
    [SwaggerOperation(
        Summary = "Consulta livro por ISBN",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Dados do livro.
        * **Status 400 (Bad Request):** ISBN inválido ou erro na consulta.
        * **Status 404 (Not Found):** ISBN não encontrado.
        * **Status 502 (Bad Gateway):** Serviço externo indisponível.

        ## Observações:
        * Requer autenticação (JWT Bearer).
        * Os dados são buscados em serviço externo a partir do ISBN informado.
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Dados do livro", type: typeof(EdicaoLivroModel))]
    [SwaggerResponse(statusCode: 400, description: "ISBN inválido ou erro na consulta", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "ISBN não encontrado", type: typeof(string))]
    [SwaggerResponse(statusCode: 502, description: "Serviço externo indisponível", type: typeof(string))]
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
    [EnableRateLimiting("politica_listagem")]
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
    [EnableRateLimiting("politica_escrita")]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra livro",
        Description = """
        ## Informações do Retorno:
        * **Status 201 (Created):** Livro criado.
        * **Status 400 (Bad Request):** Dados inválidos.
        * **Status 404 (Not Found):** Autor, categoria ou ISBN não encontrado.
        * **Status 409 (Conflict):** ISBN já cadastrado.
        * **Status 502 (Bad Gateway):** Serviço externo indisponível.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        * **AutorId** e **CategoriaId** devem referenciar registros existentes.
        * O ISBN é validado no serviço externo.
        * **Titulo** e **AnoPublicacao** são opcionais: se omitidos, são preenchidos com os dados do serviço externo.
        """
    )]
    [SwaggerRequestExample(typeof(PostLivroDto), typeof(LivroRequestSample))]
    [SwaggerResponse(statusCode: 201, description: "Livro criado", type: typeof(GetLivroDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Autor, categoria ou ISBN não encontrado", type: typeof(string))]
    [SwaggerResponse(statusCode: 409, description: "ISBN já cadastrado", type: typeof(string))]
    [SwaggerResponse(statusCode: 502, description: "Serviço externo indisponível", type: typeof(string))]
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
    [EnableRateLimiting("politica_escrita")]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Edita livro",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Livro atualizado.
        * **Status 400 (Bad Request):** Dados inválidos.
        * **Status 404 (Not Found):** Livro, autor, categoria ou ISBN não encontrado.
        * **Status 409 (Conflict):** ISBN já cadastrado em outro livro.
        * **Status 502 (Bad Gateway):** Serviço externo indisponível.

        ## Observações:
        * Requer autenticação com perfil **admin**.
        * O ISBN é validado no serviço externo.
        * **Titulo** e **AnoPublicacao** são opcionais: se omitidos, são preenchidos com os dados do serviço externo.
        """
    )]
    [SwaggerRequestExample(typeof(PostLivroDto), typeof(LivroRequestSample))]
    [SwaggerResponse(statusCode: 200, description: "Livro atualizado", type: typeof(GetLivroDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponse(statusCode: 404, description: "Livro, autor, categoria ou ISBN não encontrado", type: typeof(string))]
    [SwaggerResponse(statusCode: 409, description: "ISBN já cadastrado em outro livro", type: typeof(string))]
    [SwaggerResponse(statusCode: 502, description: "Serviço externo indisponível", type: typeof(string))]
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
    [EnableRateLimiting("politica_escrita")]
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
