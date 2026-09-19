using AssetsService.DTOs;
using AssetsService.Models;

namespace AssetsService.Services;

// Logica pura de depreciacion (sin HTTP ni BD): recibe el activo y su
// categoria, y devuelve la tabla mensual. Formulas SOLO de AGENTS.md.
public class DepreciacionService
{
    public List<FilaDepreciacion> CalcularTabla(Activo activo, Categoria categoria)
    {
        // --- Formulas oficiales de AGENTS.md ---
        // VD = VALOR_COMPRA - VALOR_COMPRA * 0.10  (el activo termina en 10% de su valor)
        decimal valorDepreciable = activo.PrecioCompra - activo.PrecioCompra * 0.10m;

        // VDA = VD / TIEMPO_DE_VIDA_UTIL (anios) ; VDM = VDA / 12
        decimal vda = valorDepreciable / categoria.VidaUtilAnios;
        decimal vdm = vda / 12;

        int vidaUtilMeses = categoria.VidaUtilAnios * 12;

        // La tabla se detiene en la fecha de corte, o antes si el activo
        // llega al fin de su vida util (valor real = 10% del precio).
        DateTime finVidaUtil = activo.FechaCompra.AddMonths(vidaUtilMeses);
        DateTime fechaLimite = activo.FechaCorte < finVidaUtil ? activo.FechaCorte : finVidaUtil;

        var filas = new List<FilaDepreciacion>();
        decimal acumulado = 0m;
        DateTime fecha = activo.FechaCompra;

        // Primera fila (mes de compra): depreciacion = 0, acumulado = 0, real = precio
        filas.Add(new FilaDepreciacion
        {
            Fecha = fecha,
            ValorDepreciacion = 0m,
            ValorDepreciacionAcumulado = 0m,
            ValorReal = activo.PrecioCompra
        });

        // Filas siguientes: salta un mes, suma VDM al acumulado y resta al precio
        fecha = fecha.AddMonths(1);
        while (fecha <= fechaLimite)
        {
            acumulado += vdm;
            filas.Add(new FilaDepreciacion
            {
                Fecha = fecha,
                ValorDepreciacion = vdm,
                ValorDepreciacionAcumulado = acumulado,
                ValorReal = activo.PrecioCompra - acumulado
            });
            fecha = fecha.AddMonths(1);
        }

        return filas;
    }
}