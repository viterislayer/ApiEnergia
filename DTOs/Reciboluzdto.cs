using System.ComponentModel.DataAnnotations;

namespace ApiEnergia.DTOs
{
    // ── Lo que el cliente manda para CREAR un recibo ──────────────────────────
    public class ReciboLuzRequest
    {
        [Required(ErrorMessage = "El número de contador es obligatorio.")]
        [MaxLength(30)]
        public string NumeroContador { get; set; } = null!;

        [Required(ErrorMessage = "El saldo pendiente es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El saldo pendiente debe ser mayor a 0.")]
        public decimal SaldoPendiente { get; set; }
    }

    // ── Para actualizar solo el saldo ─────────────────────────────────────────
    public class ReciboLuzUpdateRequest
    {
        [Required(ErrorMessage = "El saldo pendiente es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El saldo pendiente no puede ser negativo.")]
        public decimal SaldoPendiente { get; set; }
    }

    // ── Lo que la API devuelve ─────────────────────────────────────────────────
    public class ReciboLuzResponse
    {
        public int IdRecibo { get; set; }
        public string NumeroContador { get; set; } = null!;
        public decimal SaldoPendiente { get; set; }
        public DateTime FechaEmision { get; set; }
    }

    // ── Resumen de saldo total por contador ───────────────────────────────────
    public class SaldoTotalResponse
    {
        public string NumeroContador { get; set; } = null!;
        public string NombreTitular { get; set; } = null!;
        public decimal SaldoTotal { get; set; }
        public int CantidadRecibos { get; set; }
    }
}