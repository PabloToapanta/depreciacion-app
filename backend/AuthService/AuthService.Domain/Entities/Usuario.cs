using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Domain.Entities;

// Mapea la tabla Usuarios de AuthDB (equivalente a @Entity/@Table de JPA).
// Solo usa anotaciones del BCL, NO EntityFrameworkCore: Domain no depende
// de ninguna infraestructura.
[Table("Usuarios")]
public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Column("usuario")]
    public string Username { get; set; } = string.Empty;

    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("nombre_completo")]
    public string? NombreCompleto { get; set; }

    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; }
}