using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEnergia.Models
{
    [Table("recibo_luz")]
    public class ReciboLuz
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_recibo")]
        public int IdRecibo { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("numero_contador")]
        public string NumeroContador { get; set; } = null!;

        [Required]
        [Column("saldo_pendiente", TypeName = "decimal(10,2)")]
        public decimal SaldoPendiente { get; set; }

        [Required]
        [Column("fecha_emision")]
        public DateTime FechaEmision { get; set; }
    }
}