using Biblioteca.API.Doc.Samples;
using Biblioteca.Application.Dtos;
using Biblioteca.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Biblioteca.API.Controllers;

[AllowAnonymous]
[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserUseCase _userUseCase;

    public UserController(IUserUseCase userUseCase)
    {
        _userUseCase = userUseCase;
    }

    /// <summary>
    /// Cadastra usuário
    /// </summary>
    /// <returns>Usuário criado</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra usuário",
        Description = """
        ## Informações do Retorno:
        * **Status 201 (Created):** Usuário criado.
        * **Status 400 (Bad Request):** Dados inválidos.
        * **Status 409 (Conflict):** E-mail já cadastrado.

        ## Observações:
        * Endpoint público, não exige autenticação.
        * **Role** aceita `admin` ou `leitor`.
        * A senha deve ter no mínimo 6 caracteres e coincidir com **ConfirmPassword**.
        """
    )]
    [SwaggerRequestExample(typeof(PostUserDto), typeof(UserRequestSample))]
    [SwaggerResponse(statusCode: 201, description: "Usuário criado", type: typeof(GetUserDto))]
    [SwaggerResponse(statusCode: 400, description: "Dados inválidos", type: typeof(string))]
    [SwaggerResponse(statusCode: 409, description: "E-mail já cadastrado", type: typeof(string))]
    [SwaggerResponseExample(statusCode: 201, typeof(UserResponseSample))]
    public async Task<IActionResult> Post([FromBody] PostUserDto model)
    {
        var resultado = await _userUseCase.AdicionarUserAsync(model);

        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

        return StatusCode(resultado.StatusCode, resultado.Value);
    }
}
