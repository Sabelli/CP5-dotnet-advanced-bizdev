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

    [HttpGet]
    [EnableRateLimiting("politica_listagem")]
    [SwaggerOperation(Summary = "Lista autores paginados", Description = "Retorna os autores com paginação por `Deslocamento` e `RegistroRetornado`. Requer autenticação.")]
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

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obtém autor por id")]
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

    [Authorize(Roles = "admin")]
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra autor", Description = "Requer perfil `admin`.")]
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

    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Edita autor", Description = "Requer perfil `admin`.")]
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

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remove autor", Description = "Requer perfil `admin`.")]
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
