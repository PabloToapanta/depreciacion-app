using AssetsService.Domain.Entities;

namespace AssetsService.Application.DTOs;

// Respuesta completa de GET /api/activos/{id}/depreciacion:
// datos del activo + la tabla ANUAL (con posible fila final parcial).
public class TablaDepreciacionResponse
{
    public int ActivoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal PrecioCompra { get; set; }
    public DateTime FechaCompra { get; set; }
    public DateTime FechaCorte { get; set; }
    public List<FilaDepreciacion> Filas { get; set; } = new();
}