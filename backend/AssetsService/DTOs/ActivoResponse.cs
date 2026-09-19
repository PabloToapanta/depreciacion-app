namespace AssetsService.DTOs;

// Respuesta con los datos del activo guardado.
public class ActivoResponse
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioCompra { get; set; }
    public DateTime FechaCompra { get; set; }
    public DateTime FechaCorte { get; set; }
}