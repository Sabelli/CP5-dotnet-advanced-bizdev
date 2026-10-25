using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers;

[AllowAnonymous]
[Route("api/health")]
[ApiController]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Verifica a saúde da aplicação
    /// </summary>
    /// <returns>Status geral e de cada verificação</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Verifica a saúde da aplicação",
        Description = """
        ## Informações do Retorno:
        * **Status 200 (OK):** Aplicação e banco Oracle saudáveis.
        * **Status 503 (Service Unavailable):** Alguma verificação falhou.

        ## Observações:
        * Não requer autenticação.
        * Lista cada verificação (**self** e **oracle**) com status, duração e erro, se houver.
        * Os endpoints `/health/live` e `/health/db` continuam disponíveis para probes.
        """
    )]
    [SwaggerResponse(statusCode: 200, description: "Aplicação saudável")]
    [SwaggerResponse(statusCode: 503, description: "Alguma verificação falhou")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var relatorio = await _healthCheckService.CheckHealthAsync(cancellationToken);

        var resposta = new
        {
            Status = relatorio.Status.ToString(),
            DuracaoMs = relatorio.TotalDuration.TotalMilliseconds,
            Verificacoes = relatorio.Entries.Select(e => new
            {
                Nome = e.Key,
                Status = e.Value.Status.ToString(),
                DuracaoMs = e.Value.Duration.TotalMilliseconds,
                Erro = e.Value.Exception?.Message
            })
        };

        return StatusCode(relatorio.Status == HealthStatus.Healthy ? 200 : 503, resposta);
    }
}
