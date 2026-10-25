using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Controllers;

[Route("api/categoria")]
[ApiController]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaUseCase _categoriaUseCase;

    public CategoriaController(ICategoriaUseCase categoriaUseCase)
    {
        _categoriaUseCase = categoriaUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int Deslocamento, int RegistroRetornado)
    {
        var resultado = await _categoriaUseCase.ObterTodosAsync(Deslocamento, RegistroRetornado);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var resultado = await _categoriaUseCase.ObterUmAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostCategoriaDto dto)
    {
        var resultado = await _categoriaUseCase.AdicionarAsync(dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] PostCategoriaDto dto)
    {
        var resultado = await _categoriaUseCase.EditarAsync(id, dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var resultado = await _categoriaUseCase.DeletarAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
