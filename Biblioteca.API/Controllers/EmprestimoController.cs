using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Biblioteca.API.Controllers;

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
    public async Task<IActionResult> Get(int Deslocamento, int RegistroRetornado)
    {
        var resultado = await _emprestimoUseCase.ObterTodosAsync(Deslocamento, RegistroRetornado);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var resultado = await _emprestimoUseCase.ObterUmAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostEmprestimoDto dto)
    {
        var resultado = await _emprestimoUseCase.EmprestarAsync(dto);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }

    [HttpPut("{id}/devolver")]
    public async Task<IActionResult> Devolver(int id)
    {
        var resultado = await _emprestimoUseCase.DevolverAsync(id);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
