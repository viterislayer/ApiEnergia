using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEnergia.Models
{
    /// <summary>
    /// Representa al titular/cliente del servicio de energía eléctrica.
    /// Se identifica por su número de contador (medidor de luz).
    /// </summary>
    [Table("titular_servicio")]
    public class TitularServicio
    {
        [Key]
        [Column("id_titular")]
        public int IdTitular { get; set; }

        [Required]
        [StringLength(30)]
        [Column("numero_contador")]
        public string NumeroContador { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Column("nombre_responsable")]
        public string NombreResponsable { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("direccion_inmueble")]
        public string DireccionInmueble { get; set; } = string.Empty;
    }
}