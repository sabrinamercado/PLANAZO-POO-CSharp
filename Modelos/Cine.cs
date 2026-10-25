namespace Planazo.Modelos;

/// <summary>HERENCIA: Cine "es un" Evento. Mantiene el precio base.</summary>
public class Cine : Evento
{
    public Cine(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
        : base(id, nombre, fecha, precioBase, capacidadMaxima)
    {
    }

    public override string Tipo => "Cine";
    public override string DetallePrecio => "Precio base (sin cambios)";

    public override decimal CalcularPrecioFinal() => PrecioBase;
}
