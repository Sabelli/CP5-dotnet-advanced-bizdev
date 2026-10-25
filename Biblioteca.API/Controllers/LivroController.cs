using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Controllers;

[Route("api/livro")]
[ApiController]
public class LivroController : ControllerBase
{
    private readonly ILivroUseCase _livroUseCase;

    public LivroController(ILivroUseCase livroUseCase)
    {
        _livroUseCase = livroUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int Deslocamento, int RegistroRetornado)
    {
        var resultado = await _livroUseCase.ObterTodosAsync(Deslocamento, RegistroRetornado);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var resultado = await _livroUseCase.ObterUmAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostLivroDto dto)
    {
        var resultado = await _livroUseCase.AdicionarAsync(dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] PostLivroDto dto)
    {
        var resultado = await _livroUseCase.EditarAsync(id, dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var resultado = await _livroUseCase.DeletarAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
