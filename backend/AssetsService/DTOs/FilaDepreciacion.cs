namespace AssetsService.DTOs;

// Una fila de la tabla mensual de depreciacion:
// Fecha | Valor Depreciacion | Valor Depreciacion Acumulado | Valor Real
public class FilaDepreciacion
{
    public DateTime Fecha { get; set; }
    public decimal ValorDepreciacion { get; set; }
    public decimal ValorDepreciacionAcumulado { get; set; }
    public decimal ValorReal { get; set; }
}