using AssetsService.Domain.Entities;
using AssetsService.Domain.Services;

namespace DepreciacionApp.Tests;

// Tests de la formula CORREGIDA de AGENTS.md: tabla ANUAL con resto parcial
// en meses completos. Los tests viejos (tabla mensual) ya no aplican.
public class DepreciacionServiceTests
{
    private static readonly Categoria Tecnologia = new() { Id = 1, Nombre = "Tecnología", VidaUtilAnios = 3 };
    private static readonly Categoria Edificios = new() { Id = 4, Nombre = "Edificios", VidaUtilAnios = 20 };

    // Ejemplo oficial 1 de AGENTS.md: Tecnologia $900, compra 22/09/2026, corte 03/08/2027.
    // VDA = (900 * 0.9) / 3 = 270 ; VDM = 22.5.
    // No cabe ni un anio completo; 10 meses completos -> fila parcial 10 * 22.5 = 225.
    [Fact]
    public void Tecnologia_900_Compra_22_09_2026_Corte_03_08_2027_Genera_Fila_Inicial_Y_Parcial()
    {
        var activo = new Activo
        {
            PrecioCompra = 900m,
            FechaCompra = new DateTime(2026, 9, 22),
            FechaCorte = new DateTime(2027, 8, 3)
        };

        var filas = new DepreciacionService().CalcularTabla(activo, Tecnologia);

        Assert.Equal(2, filas.Count);

        // Fila inicial
        Assert.Equal(new DateTime(2026, 9, 22), filas[0].Fecha);
        Assert.Equal(0m, filas[0].ValorDepreciacion);
        Assert.Equal(0m, filas[0].ValorDepreciacionAcumulado);
        Assert.Equal(900m, filas[0].ValorReal);

        // Fila parcial final
        Assert.Equal(new DateTime(2027, 8, 3), filas[1].Fecha);
        Assert.Equal(225m, filas[1].ValorDepreciacion);
        Assert.Equal(225m, filas[1].ValorDepreciacionAcumulado);
        Assert.Equal(675m, filas[1].ValorReal);
    }

    // Ejemplo oficial 2 de AGENTS.md: Edificios $100000, compra 01/01/2023, corte 01/06/2026.
    // VDA = (100000 * 0.9) / 20 = 4500 ; VDM = 375.
    // 3 anios completos (2024, 2025, 2026) + resto de 5 meses -> 5 * 375 = 1875.
    [Fact]
    public void Edificios_100000_Compra_01_01_2023_Corte_01_06_2026_Genera_3_Anios_Y_Parcial()
    {
        var activo = new Activo
        {
            PrecioCompra = 100000m,
            FechaCompra = new DateTime(2023, 1, 1),
            FechaCorte = new DateTime(2026, 6, 1)
        };

        var filas = new DepreciacionService().CalcularTabla(activo, Edificios);

        Assert.Equal(5, filas.Count);

        // Fila inicial
        Assert.Equal(new DateTime(2023, 1, 1), filas[0].Fecha);
        Assert.Equal(0m, filas[0].ValorDepreciacion);
        Assert.Equal(0m, filas[0].ValorDepreciacionAcumulado);
        Assert.Equal(100000m, filas[0].ValorReal);

        // 3 anios completos de 4500
        for (int i = 1; i <= 3; i++)
        {
            Assert.Equal(new DateTime(2023 + i, 1, 1), filas[i].Fecha);
            Assert.Equal(4500m, filas[i].ValorDepreciacion);
            Assert.Equal(4500m * i, filas[i].ValorDepreciacionAcumulado);
            Assert.Equal(100000m - 4500m * i, filas[i].ValorReal);
        }

        // Fila parcial final: 5 meses * 375 = 1875
        Assert.Equal(new DateTime(2026, 6, 1), filas[4].Fecha);
        Assert.Equal(1875m, filas[4].ValorDepreciacion);
        Assert.Equal(15375m, filas[4].ValorDepreciacionAcumulado);
        Assert.Equal(84625m, filas[4].ValorReal);
    }

    // Caso borde: corte en aniversario exacto -> NO hay fila parcial.
    // Tecnologia $900, compra 22/09/2026, corte 22/09/2027 -> 1 anio completo de 270.
    [Fact]
    public void Corte_En_Aniversario_Exacto_No_Genera_Fila_Parcial()
    {
        var activo = new Activo
        {
            PrecioCompra = 900m,
            FechaCompra = new DateTime(2026, 9, 22),
            FechaCorte = new DateTime(2027, 9, 22)
        };

        var filas = new DepreciacionService().CalcularTabla(activo, Tecnologia);

        Assert.Equal(2, filas.Count);
        Assert.Equal(new DateTime(2027, 9, 22), filas[1].Fecha);
        Assert.Equal(270m, filas[1].ValorDepreciacion);
        Assert.Equal(270m, filas[1].ValorDepreciacionAcumulado);
        Assert.Equal(630m, filas[1].ValorReal);
    }

    // Caso borde: corte despues del fin de vida util -> la tabla se detiene en
    // finVidaUtil (valor real = 10% del precio), sin fila parcial.
    // Tecnologia $900, compra 22/09/2026, corte 22/09/2030 -> finVidaUtil 22/09/2029.
    [Fact]
    public void Corte_Despues_De_Fin_De_VidaUtil_Se_Detiene_En_FinVidaUtil()
    {
        var activo = new Activo
        {
            PrecioCompra = 900m,
            FechaCompra = new DateTime(2026, 9, 22),
            FechaCorte = new DateTime(2030, 9, 22)
        };

        var filas = new DepreciacionService().CalcularTabla(activo, Tecnologia);

        Assert.Equal(4, filas.Count); // inicial + 3 anios completos
        Assert.Equal(new DateTime(2029, 9, 22), filas[3].Fecha);
        Assert.Equal(270m, filas[3].ValorDepreciacion);
        Assert.Equal(810m, filas[3].ValorDepreciacionAcumulado);
        Assert.Equal(90m, filas[3].ValorReal); // 10% del precio
    }
}