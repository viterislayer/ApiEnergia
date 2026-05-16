using System.ComponentModel.DataAnnotations;

namespace ApiEnergia.DTOs
{
    /// <summary>
    /// Body para crear un pago.
    /// canalPago acepta: "OFICINA", "BANCA_EN_LINEA", "APP_MOVIL", "AGENTE_AUTORIZADO"
    /// </summary>
    public class PagoRequest
    {
        [Required(ErrorMessage = "El número de contador es obligatorio.")]
        [MaxLength(30)]
        public string NumeroContador { get; set; } = null!;

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }

        /// <summary>
        /// Canal de pago. Valores válidos: OFICINA | BANCA_EN_LINEA | APP_MOVIL | AGENTE_AUTORIZADO
        /// </summary>
        [Required(ErrorMessage = "El canal de pago es obligatorio.")]
        public string CanalPago { get; set; } = null!;
    }

    /// <summary>
    /// Body para actualizar canal y código de un pago existente.
    /// </summary>
    public class PagoUpdateRequest
    {
        /// <summary>
        /// Canal de pago. Valores válidos: OFICINA | BANCA_EN_LINEA | APP_MOVIL | AGENTE_AUTORIZADO
        /// </summary>
        [Required(ErrorMessage = "El canal de pago es obligatorio.")]
        public string CanalPago { get; set; } = null!;

        [Required(ErrorMessage = "El código de autorización es obligatorio.")]
        [MinLength(6)]
        [MaxLength(50)]
        public string CodigoAutorizacionBanco { get; set; } = null!;
    }

    /// <summary>
    /// Lo que devuelve la API en todas las respuestas de pagos.
    /// </summary>
    public class PagoResponse
    {
        public int IdPago { get; set; }
        public string NumeroContador { get; set; } = null!;
        public decimal Monto { get; set; }
        public DateTime FechaCobro { get; set; }
        public string CanalPago { get; set; } = null!;
        public string? CodigoAutorizacionBanco { get; set; }
    }

    // Canales válidos para validar en el controller
    public static class CanalesPago
    {
        public static readonly string[] Validos =
        {
            "OFICINA",
            "BANCA_EN_LINEA",
            "APP_MOVIL",
            "AGENTE_AUTORIZADO"
        };

        public static bool EsValido(string canal) =>
            Validos.Contains(canal?.ToUpper().Trim());
    }
}