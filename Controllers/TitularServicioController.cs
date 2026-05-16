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
    [Tags("1. TitularServicio - Registrar Titular")]
    public class TitularServicioController : ControllerBase
    {
        private readonly EnergiaDbContext _db;
        public TitularServicioController(EnergiaDbContext db) => _db = db;

        // ── GET api/TitularServicio ───────────────────────────────────────
        // Devuelve todos los titulares ordenados por nombre
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TitularServicioResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var titulares = await _db.TitularServicio
                .OrderBy(t => t.NombreResponsable)
                .Select(t => new TitularServicioResponse
                {
                    IdTitular = t.IdTitular,
                    NumeroContador = t.NumeroContador,
                    NombreResponsable = t.NombreResponsable,
                    DireccionInmueble = t.DireccionInmueble
                })
                .ToListAsync();

            return Ok(titulares);
        }

        // ── POST api/TitularServicio ──────────────────────────────────────
        // Registra un nuevo titular. El numero_contador debe ser único.
        [HttpPost]
        [ProducesResponseType(typeof(TitularServicioResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] TitularServicioRequest req)
        {
            bool existe = await _db.TitularServicio
                .AnyAsync(t => t.NumeroContador == req.NumeroContador);

            if (existe)
                return Conflict(new { mensaje = $"El contador {req.NumeroContador} ya está registrado." });

            var titular = new TitularServicio
            {
                NumeroContador = req.NumeroContador,
                NombreResponsable = req.NombreResponsable,
                DireccionInmueble = req.DireccionInmueble
            };

            _db.TitularServicio.Add(titular);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = titular.IdTitular }, new TitularServicioResponse
            {
                IdTitular = titular.IdTitular,
                NumeroContador = titular.NumeroContador,
                NombreResponsable = titular.NombreResponsable,
                DireccionInmueble = titular.DireccionInmueble
            });
        }

        // ── PUT api/TitularServicio/{id} ──────────────────────────────────
        // Actualiza nombre y dirección. El numero_contador NO se puede cambiar.
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TitularServicioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] TitularServicioRequest req)
        {
            var titular = await _db.TitularServicio.FindAsync(id);
            if (titular is null)
                return NotFound(new { mensaje = $"Titular con id {id} no encontrado." });

            titular.NombreResponsable = req.NombreResponsable;
            titular.DireccionInmueble = req.DireccionInmueble;

            await _db.SaveChangesAsync();

            return Ok(new TitularServicioResponse
            {
                IdTitular = titular.IdTitular,
                NumeroContador = titular.NumeroContador,
                NombreResponsable = titular.NombreResponsable,
                DireccionInmueble = titular.DireccionInmueble
            });
        }

        // ── DELETE api/TitularServicio/{id} ───────────────────────────────
        // Elimina el titular Y todos sus recibos y pagos asociados (cascada)
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var titular = await _db.TitularServicio.FindAsync(id);
            if (titular is null)
                return NotFound(new { mensaje = $"Titular con id {id} no encontrado." });

            // Cascada: eliminar todos los pagos del contador
            var pagos = _db.PagosProcesados
                .Where(p => p.NumeroContador == titular.NumeroContador);
            _db.PagosProcesados.RemoveRange(pagos);

            // Cascada: eliminar todos los recibos del contador
            var recibos = _db.ReciboLuz
                .Where(r => r.NumeroContador == titular.NumeroContador);
            _db.ReciboLuz.RemoveRange(recibos);

            _db.TitularServicio.Remove(titular);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}