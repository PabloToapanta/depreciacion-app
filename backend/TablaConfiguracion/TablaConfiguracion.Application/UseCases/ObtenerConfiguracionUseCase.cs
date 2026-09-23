using TablaConfiguracion.Application.DTOs;
using TablaConfiguracion.Application.Interfaces;

namespace TablaConfiguracion.Application.UseCases;

// Orquesta la lectura de la configuracion del sistema.
public class ObtenerConfiguracionUseCase
{
    private readonly IConfiguracionRepository _repository;

    public ObtenerConfiguracionUseCase(IConfiguracionRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConfiguracionResponse?> EjecutarAsync()
    {
        var configuracion = await _repository.ObtenerAsync();
        if (configuracion is null)
            return null;

        return new ConfiguracionResponse
        {
            NombreSistema = configuracion.NombreSistema,
            NombresApellidos = configuracion.NombresApellidos,
            Version = configuracion.Version,
            Fecha = configuracion.Fecha
        };
    }
}