using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetsService.Models;

// Mapea la tabla Activos de AssetsDB (el "detalle" del master-detail).
[Table("Activos")]
public class Activo
{
    [Key]
    public int Id { get; set; }

    [Column("categoria_id")]
    public int CategoriaId { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("precio_compra")]
    public decimal PrecioCompra { get; set; }

    [Column("fecha_compra")]
    public DateTime FechaCompra { get; set; }

    [Column("fecha_corte")]
    public DateTime FechaCorte { get; set; }

    [Column("fecha_registro")]
    public DateTime FechaRegistro { get; set; }

    // Propiedad de navegacion: permite llegar a la categoria (y su vida util)
    // desde el activo, como un JOIN de SQL pero con objetos.
    [ForeignKey("CategoriaId")]
    public Categoria? Categoria { get; set; }
}