using ApiEnergia.DTOs;
using ApiEnergia.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiEnergia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("3. Pagos")]
    public class PagosController : ControllerBase
    {
        private readonly IEnergiaService _energiaService;

        public PagosController(IEnergiaService energiaService)
        {
            _energiaService = energiaService;
        }

        // ── GET api/Pagos ─────────────────────────────────────────────────────
        // Devuelve todos los pagos del más reciente al más antiguo
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            return Ok();
        }

        // ── POST api/Pagos ────────────────────────────────────────────────────
        // Registra un nuevo pago procesado.
        // El cliente manda SOLO: numeroContador, monto, canalPago
        // El sistema genera AUTOMATICAMENTE: idPago, fechaCobro, codigoAutorizacionBanco
        //
        // canalPago valores válidos:
        //   "OFICINA" | "BANCA_EN_LINEA" | "APP_MOVIL" | "AGENTE_AUTORIZADO"
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> Create([FromBody] PagoRequest req)
        {
            if (!CanalesPago.EsValido(req.CanalPago))
                return BadRequest(new
                {
                    mensaje = "Canal de pago inválido.",
                    valoresValidos = CanalesPago.Validos
                });

            await _energiaService.ProcesarPagoExternoAsync(req.NumeroContador, req.Monto);
            return StatusCode(StatusCodes.Status201Created);
        }

        // ── PUT api/Pagos/{id} ────────────────────────────────────────────────
        // Corrección administrativa: actualiza canal y código de autorización
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(int id, [FromBody] PagoUpdateRequest req)
        {
            if (!CanalesPago.EsValido(req.CanalPago))
                return BadRequest(new
                {
                    mensaje = "Canal de pago inválido.",
                    valoresValidos = CanalesPago.Validos
                });

            return Ok();
        }

        // ── DELETE api/Pagos/{id} ─────────────────────────────────────────────
        // Elimina un pago por ID
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            return NoContent();
        }
    }
}