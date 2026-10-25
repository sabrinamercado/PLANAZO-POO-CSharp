namespace Planazo.Modelos;

/// <summary>Taller: aplica un descuento del 10 %.</summary>
public class Taller : Evento
{
    private const decimal Descuento = 0.10m;

    public Taller(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
        : base(id, nombre, fecha, precioBase, capacidadMaxima)
    {
    }

    public override string Tipo => "Taller";
    public override string DetallePrecio => "Descuento del 10 %";

    public override decimal CalcularPrecioFinal() => PrecioBase * (1 - Descuento);
}
