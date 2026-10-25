using System.Globalization;

namespace Planazo.Utilidades;

/// <summary>Funciones de ayuda para mostrar dinero y fechas con formato colombiano.</summary>
public static class Formato
{
    private static readonly CultureInfo Cultura = new("es-CO");

    public static string Moneda(decimal valor) => valor.ToString("C0", Cultura);

    public static string Fecha(DateTime fecha) => fecha.ToString("dd/MM/yyyy HH:mm", Cultura);
}
