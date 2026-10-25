namespace Planazo.Modelos;

/// <summary>Concierto: aumenta el precio base un 15 %.</summary>
public class Concierto : Evento
{
    private const decimal Recargo = 0.15m;

    public Concierto(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
        : base(id, nombre, fecha, precioBase, capacidadMaxima)
    {
    }

    public override string Tipo => "Concierto";
    public override string DetallePrecio => "Recargo del 15 %";

    // 50.000 * 1,15 = 57.500
    public override decimal CalcularPrecioFinal() => PrecioBase * (1 + Recargo);
}
