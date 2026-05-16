using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEnergia.Models
{
    [Table("pagos_procesados")]
    public class PagosProcesados
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_pago")]
        public int IdPago { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("numero_contador")]
        public string NumeroContador { get; set; } = null!;

        [Required]
        [Column("monto", TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        [Required]
        [Column("fecha_cobro")]
        public DateTime FechaCobro { get; set; }

        // Se guarda como string en la BD: "OFICINA", "BANCA_EN_LINEA", etc.
        [Required]
        [MaxLength(30)]
        [Column("canal_pago")]
        public string CanalPago { get; set; } = null!;

        [MaxLength(50)]
        [Column("codigo_autorizacion_banco")]
        public string? CodigoAutorizacionBanco { get; set; }
    }
}