namespace AssetsService.Application.DTOs;

// Cuerpo que el cliente envia en POST /api/activos.
public class ActivoRequest
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioCompra { get; set; }
    public DateTime FechaCompra { get; set; }
    public DateTime FechaCorte { get; set; }
}