using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TablaConfiguracion.Domain.Entities;

// Mapea la tabla configuracion de ConfigDB: contiene los datos del sistema
// (nombre, desarrolladores, version y fecha) que el microservicio expone.
[Table("configuracion")]
public class Configuracion
{
    [Key]
    public int Id { get; set; }

    [Column("nombre_sistema")]
    public string NombreSistema { get; set; } = string.Empty;

    [Column("nombres_apellidos")]
    public string NombresApellidos { get; set; } = string.Empty;

    [Column("version")]
    public string Version { get; set; } = string.Empty;

    [Column("fecha")]
    public DateTime Fecha { get; set; }
}