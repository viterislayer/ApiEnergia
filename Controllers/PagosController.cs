using ApiEnergia.DbContext;
using ApiEnergia.DTOs;
using ApiEnergia.Models;
using ApiEnergia.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiEnergia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("3. Pagos")]
    public class PagosController : ControllerBase
    {
        private readonly EnergiaDbContext _db;
        private readonly BancoService _banco;

        public PagosController(EnergiaDbContext db, BancoService banco)
        {
            _db = db;
            _banco = banco;
        }

        // ── GET api/Pagos ─────────────────────────────────────────────────────
        // Devuelve todos los pagos del más reciente al más antiguo
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PagoResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var pagos = await _db.PagosProcesados
                .OrderByDescending(p => p.FechaCobro)
                .Select(p => new PagoResponse
                {
                    IdPago = p.IdPago,
                    NumeroContador = p.NumeroContador,
                    Monto = p.Monto,
                    FechaCobro = p.FechaCobro,
                    CanalPago = p.CanalPago,
                    CodigoAutorizacionBanco = p.CodigoAutorizacionBanco
                })
                .ToListAsync();

            return Ok(pagos);
        }

        // ── POST api/Pagos ────────────────────────────────────────────────────
        // Registra un nuevo pago procesado.
        // El cliente manda SOLO: numeroContador, monto, canalPago
        // El sistema genera AUTOMATICAMENTE: idPago, fechaCobro, codigoAutorizacionBanco
        //
        // canalPago valores válidos:
        //   "OFICINA" | "BANCA_EN_LINEA" | "APP_MOVIL" | "AGENTE_AUTORIZADO"
        [HttpPost]
        [ProducesResponseType(typeof(PagoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> Create([FromBody] PagoRequest req)
        {
            // 1. Validar canal de pago
            var canal = req.CanalPago?.ToUpper().Trim();
            if (!CanalesPago.EsValido(canal!))
                return BadRequest(new
                {
                    mensaje = "Canal de pago inválido.",
                    valoresValidos = CanalesPago.Validos
                });

            // 2. Validar que el contador esté registrado en titular_servicio
            bool contadorExiste = await _db.TitularServicio
                .AnyAsync(t => t.NumeroContador == req.NumeroContador);
            if (!contadorExiste)
                return BadRequest(new
                {
                    mensaje = $"El contador '{req.NumeroContador}' no está registrado.",
                    solucion = "Primero cree el titular en POST /api/TitularServicio"
                });

            // 3. Autorizar pago con el banco (genera código automático)
            var codigo = await _banco.AutorizarPagoAsync(req.Monto, req.NumeroContador);
            if (codigo is null)
                return StatusCode(StatusCodes.Status502BadGateway,
                    new { mensaje = "El servicio bancario rechazó la transacción." });

            // 4. Guardar en BD
            var pago = new PagosProcesados
            {
                NumeroContador = req.NumeroContador,
                Monto = req.Monto,
                CanalPago = canal!,
                FechaCobro = DateTime.UtcNow,
                CodigoAutorizacionBanco = codigo
            };

            _db.PagosProcesados.Add(pago);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = pago.IdPago }, new PagoResponse
            {
                IdPago = pago.IdPago,
                NumeroContador = pago.NumeroContador,
                Monto = pago.Monto,
                FechaCobro = pago.FechaCobro,
                CanalPago = pago.CanalPago,
                CodigoAutorizacionBanco = pago.CodigoAutorizacionBanco
            });
        }

        // ── PUT api/Pagos/{id} ────────────────────────────────────────────────
        // Corrección administrativa: actualiza canal y código de autorización
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(PagoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] PagoUpdateRequest req)
        {
            var canal = req.CanalPago?.ToUpper().Trim();
            if (!CanalesPago.EsValido(canal!))
                return BadRequest(new
                {
                    mensaje = "Canal de pago inválido.",
                    valoresValidos = CanalesPago.Validos
                });

            var pago = await _db.PagosProcesados.FindAsync(id);
            if (pago is null)
                return NotFound(new { mensaje = $"Pago con id {id} no encontrado." });

            pago.CanalPago = canal!;
            pago.CodigoAutorizacionBanco = req.CodigoAutorizacionBanco;

            await _db.SaveChangesAsync();

            return Ok(new PagoResponse
            {
                IdPago = pago.IdPago,
                NumeroContador = pago.NumeroContador,
                Monto = pago.Monto,
                FechaCobro = pago.FechaCobro,
                CanalPago = pago.CanalPago,
                CodigoAutorizacionBanco = pago.CodigoAutorizacionBanco
            });
        }

        // ── DELETE api/Pagos/{id} ─────────────────────────────────────────────
        // Elimina un pago por ID
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var pago = await _db.PagosProcesados.FindAsync(id);
            if (pago is null)
                return NotFound(new { mensaje = $"Pago con id {id} no encontrado." });

            _db.PagosProcesados.Remove(pago);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}