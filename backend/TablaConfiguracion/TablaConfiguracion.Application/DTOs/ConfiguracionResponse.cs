namespace TablaConfiguracion.Application.DTOs;

// Respuesta de la configuracion del sistema (lo que muestra la tabla del frontend).
public class ConfiguracionResponse
{
    public string NombreSistema { get; set; } = string.Empty;
    public string NombresApellidos { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}