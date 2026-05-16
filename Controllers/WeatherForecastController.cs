using Microsoft.AspNetCore.Mvc;
using ApiDocs = Microsoft.AspNetCore.Mvc.ApiExplorerSettingsAttribute;

namespace ApiEnergia.Controllers
{
    // Este controlador viene por defecto con la plantilla de Visual Studio.
    // Se oculta de la documentación de la API con ApiExplorerSettings.
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Endpoint de prueba - no pertenece al proyecto.");
    }
}