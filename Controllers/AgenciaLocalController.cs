using ApiEnergia.DTOs;
using ApiEnergia.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ApiEnergia.Controllers
{
    [ApiController]
    [Route("api/Energia/Agencia")]
    [Produces("application/json")]
    [Tags("Agencia Local")]
    [Authorize]
    public class AgenciaLocalController : ControllerBase
    {
        private readonly IEnergiaService _energiaService;

        public AgenciaLocalController(IEnergiaService energiaService)
        {
            _energiaService = energiaService;
        }

        [HttpPost("lectura")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegistrarLectura([FromBody] RegistrarLecturaDto dto)
        {
            var recibo = await _energiaService.RegistrarLecturaAsync(dto.NumeroContador, dto.Kilovatios);
            return Ok(new ConsultarDeudaResponseDto(recibo.NumeroContador, recibo.SaldoPendiente));
        }

        [HttpPost("pago-efectivo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PagoEfectivo([FromBody] PagoEfectivoAgenciaDto dto)
        {
            await _energiaService.ProcesarPagoEfectivoAsync(dto.NumeroContador, dto.MontoRecibido);
            return Ok();
        }
    }
}
