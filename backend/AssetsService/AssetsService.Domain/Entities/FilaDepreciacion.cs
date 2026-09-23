namespace AssetsService.Domain.Entities;

// Una fila de la tabla ANUAL de depreciacion:
// Fecha | Valor Depreciacion | Valor Depreciacion Acumulado | Valor Real
// Es un resultado de la regla de negocio (DepreciacionService), por eso vive
// en Domain y no en Application: Domain no puede depender de Application.
public class FilaDepreciacion
{
    public DateTime Fecha { get; set; }
    public decimal ValorDepreciacion { get; set; }
    public decimal ValorDepreciacionAcumulado { get; set; }
    public decimal ValorReal { get; set; }
}