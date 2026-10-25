using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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
    public async Task<IActionResult> Get(int Deslocamento, int RegistroRetornado)
    {
        var resultado = await _autorUseCase.ObterTodosAsync(Deslocamento, RegistroRetornado);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var resultado = await _autorUseCase.ObterUmAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostAutorDto dto)
    {
        var resultado = await _autorUseCase.AdicionarAsync(dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] PostAutorDto dto)
    {
        var resultado = await _autorUseCase.EditarAsync(id, dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var resultado = await _autorUseCase.DeletarAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
