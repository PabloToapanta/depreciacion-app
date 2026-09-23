using TablaConfiguracion.Domain.Entities;

namespace TablaConfiguracion.Application.Interfaces;

// Puerto de persistencia de la configuracion del sistema (la tabla `configuracion`).
public interface IConfiguracionRepository
{
    Task<Configuracion?> ObtenerAsync();
}