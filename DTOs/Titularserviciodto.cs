using System.ComponentModel.DataAnnotations;

namespace ApiEnergia.DTOs
{
    // ── Lo que el cliente manda para CREAR o EDITAR un titular ────────────────
    public class TitularServicioRequest
    {
        [Required(ErrorMessage = "El número de contador es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El número de contador no puede superar 30 caracteres.")]
        public string NumeroContador { get; set; } = null!;

        [Required(ErrorMessage = "El nombre del responsable es obligatorio.")]
        [MaxLength(150, ErrorMessage = "El nombre no puede superar 150 caracteres.")]
        public string NombreResponsable { get; set; } = null!;

        [Required(ErrorMessage = "La dirección del inmueble es obligatoria.")]
        [MaxLength(255, ErrorMessage = "La dirección no puede superar 255 caracteres.")]
        public string DireccionInmueble { get; set; } = null!;
    }

    // ── Lo que la API devuelve ─────────────────────────────────────────────────
    public class TitularServicioResponse
    {
        public int IdTitular { get; set; }
        public string NumeroContador { get; set; } = null!;
        public string NombreResponsable { get; set; } = null!;
        public string DireccionInmueble { get; set; } = null!;
    }
}