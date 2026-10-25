using Biblioteca.API.Doc.Samples;
using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Biblioteca.API.Controllers;

[AllowAnonymous]
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserUseCase _userUseCase;

    public AuthController(IUserUseCase userUseCase)
    {
        _userUseCase = userUseCase;
    }

    /// <summary>
    /// Autentica usuário
    /// </summary>
    /// <returns>Autenticado</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Autentica usuário",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Autenticado.
        * **Status 400 (Bad Request):** Dados inválidos.
        * **Status 401 (Unauthorized):** E-mail ou senha incorretos.

        ## Observações:
        * Endpoint público, não exige autenticação.
        * Use o JWT retornado no botão **Authorize** como `Bearer {token}`.
        """
    )]
    [SwaggerRequestExample(typeof(PostAuthDto), typeof(AuthRequestSample))]
    [SwaggerResponse(statusCode: 200, description: "Autenticado", type: typeof(GetAuthDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponse(statusCode: 401, description: "E-mail ou senha incorretos", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 200, typeof(AuthResponseSample))]
    public async Task<IActionResult> Post([FromBody] PostAuthDto model)
    {
        var resultado = await _userUseCase.AutenticaUserAsync(model);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
