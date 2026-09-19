using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetsService.Models;

// Mapea la tabla Categorias de AssetsDB (el "maestro" del master-detail).
[Table("Categorias")]
public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("vida_util_anios")]
    public int VidaUtilAnios { get; set; }
}