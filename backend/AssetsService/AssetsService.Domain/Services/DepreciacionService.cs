using AssetsService.Domain.Entities;

namespace AssetsService.Domain.Services;

// Logica pura de depreciacion (sin HTTP ni BD): recibe el activo y su
// categoria, y devuelve la tabla ANUAL con posible fila final parcial.
// Formulas SOLO de AGENTS.md (seccion "Formula de depreciacion (corregida)").
public class DepreciacionService
{
    public List<FilaDepreciacion> CalcularTabla(Activo activo, Categoria categoria)
    {
        // --- Formulas oficiales de AGENTS.md ---
        // VD = VALOR_COMPRA - VALOR_COMPRA * 0.10  (el activo termina en 10% de su valor)
        decimal valorDepreciable = activo.PrecioCompra - activo.PrecioCompra * 0.10m;

        // VDA = VD / VIDA_UTIL_ANIOS (depreciacion por anio completo)
        // VDM = VDA / 12 (depreciacion por mes, SOLO para la fila parcial final)
        decimal vda = valorDepreciable / categoria.VidaUtilAnios;
        decimal vdm = vda / 12;

        // 1. finVidaUtil = fechaCompra.AddYears(vidaUtilAnios)
        DateTime finVidaUtil = activo.FechaCompra.AddYears(categoria.VidaUtilAnios);

        // 2. limite = min(fechaCorte, finVidaUtil)
        DateTime limite = activo.FechaCorte < finVidaUtil ? activo.FechaCorte : finVidaUtil;

        var filas = new List<FilaDepreciacion>();
        decimal acumulado = 0m;

        // 3. Fila inicial (fecha de compra): depreciacion = 0, acumulado = 0, real = precio
        filas.Add(new FilaDepreciacion
        {
            Fecha = activo.FechaCompra,
            ValorDepreciacion = 0m,
            ValorDepreciacionAcumulado = 0m,
            ValorReal = activo.PrecioCompra
        });

        // 4. Para cada aniversario n = 1, 2, 3... (fechaCompra.AddYears(n)) que sea <= limite:
        //    fila de anio completo con VDA. Si el aniversario coincide exactamente con
        //    limite, terminar (no hay resto parcial).
        DateTime ultimoAniversario = activo.FechaCompra;
        for (int n = 1; ; n++)
        {
            DateTime aniversario = activo.FechaCompra.AddYears(n);
            if (aniversario > limite) break;

            acumulado += vda;
            filas.Add(new FilaDepreciacion
            {
                Fecha = aniversario,
                ValorDepreciacion = vda,
                ValorDepreciacionAcumulado = acumulado,
                ValorReal = activo.PrecioCompra - acumulado
            });
            ultimoAniversario = aniversario;

            if (aniversario == limite) return filas;
        }

        // 5. Si sobra un periodo menor a un anio entre el ultimo aniversario registrado
        //    (o la fecha de compra si no hubo ningun anio completo) y limite: contar los
        //    MESES COMPLETOS de calendario y agregar la fila parcial con meses * VDM.
        int meses = ContarMesesCompletos(ultimoAniversario, limite);
        if (meses > 0)
        {
            decimal depreciacionParcial = meses * vdm;
            acumulado += depreciacionParcial;
            filas.Add(new FilaDepreciacion
            {
                Fecha = limite,
                ValorDepreciacion = depreciacionParcial,
                ValorDepreciacionAcumulado = acumulado,
                ValorReal = activo.PrecioCompra - acumulado
            });
        }

        return filas;
    }

    // Conteo de meses completos entre dos fechas (AGENTS.md):
    // meses = (hasta.Year - desde.Year) * 12 + (hasta.Month - desde.Month)
    // si hasta.Day < desde.Day: meses = meses - 1
    private static int ContarMesesCompletos(DateTime desde, DateTime hasta)
    {
        int meses = (hasta.Year - desde.Year) * 12 + (hasta.Month - desde.Month);
        if (hasta.Day < desde.Day) meses--;
        return meses;
    }
}