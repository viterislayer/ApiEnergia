using ApiEnergia.DbContext;
using ApiEnergia.DTOs;
using ApiEnergia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiEnergia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("2. ReciboLuz")]
    public class ReciboLuzController : ControllerBase
    {
        private readonly EnergiaDbContext _db;
        public ReciboLuzController(EnergiaDbContext db) => _db = db;

        // ── GET api/ReciboLuz ─────────────────────────────────────────────
        // Lista todos los recibos, del más reciente al más antiguo
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReciboLuzResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var recibos = await _db.ReciboLuz
                .OrderByDescending(r => r.FechaEmision)
                .Select(r => new ReciboLuzResponse
                {
                    IdRecibo = r.IdRecibo,
                    NumeroContador = r.NumeroContador,
                    SaldoPendiente = r.SaldoPendiente,
                    FechaEmision = r.FechaEmision
                })
                .ToListAsync();

            return Ok(recibos);
        }

        // ── POST api/ReciboLuz ────────────────────────────────────────────
        // Emite un nuevo recibo. La fecha_emision se asigna automáticamente.
        [HttpPost]
        [ProducesResponseType(typeof(ReciboLuzResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ReciboLuzRequest req)
        {
            bool contadorExiste = await _db.TitularServicio
                .AnyAsync(t => t.NumeroContador == req.NumeroContador);

            if (!contadorExiste)
                return BadRequest(new { mensaje = $"El contador {req.NumeroContador} no está registrado." });

            var recibo = new ReciboLuz
            {
                NumeroContador = req.NumeroContador,
                SaldoPendiente = req.SaldoPendiente,
                FechaEmision = DateTime.UtcNow
            };

            _db.ReciboLuz.Add(recibo);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = recibo.IdRecibo }, new ReciboLuzResponse
            {
                IdRecibo = recibo.IdRecibo,
                NumeroContador = recibo.NumeroContador,
                SaldoPendiente = recibo.SaldoPendiente,
                FechaEmision = recibo.FechaEmision
            });
        }

        // ── PUT api/ReciboLuz/{id} ────────────────────────────────────────
        // Actualiza el saldo pendiente de un recibo (ej: pago parcial)
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ReciboLuzResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ReciboLuzUpdateRequest req)
        {
            var recibo = await _db.ReciboLuz.FindAsync(id);
            if (recibo is null)
                return NotFound(new { mensaje = $"Recibo con id {id} no encontrado." });

            recibo.SaldoPendiente = req.SaldoPendiente;
            await _db.SaveChangesAsync();

            return Ok(new ReciboLuzResponse
            {
                IdRecibo = recibo.IdRecibo,
                NumeroContador = recibo.NumeroContador,
                SaldoPendiente = recibo.SaldoPendiente,
                FechaEmision = recibo.FechaEmision
            });
        }

        // ── DELETE api/ReciboLuz/{id} ─────────────────────────────────────
        // Elimina un recibo por ID
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var recibo = await _db.ReciboLuz.FindAsync(id);
            if (recibo is null)
                return NotFound(new { mensaje = $"Recibo con id {id} no encontrado." });

            _db.ReciboLuz.Remove(recibo);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}