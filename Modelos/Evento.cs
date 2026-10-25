namespace Planazo.Modelos;

/// <summary>
/// Clase ABSTRACTA: define lo que tienen en común todos los eventos,
/// pero no se puede instanciar directamente (no existe un "Evento" genérico).
/// </summary>
public abstract class Evento
{
    // ENCAPSULAMIENTO: los datos solo se pueden leer desde afuera.
    // El único que puede modificarlos es la propia clase.
    public int Id { get; }
    public string Nombre { get; }
    public DateTime Fecha { get; }
    public decimal PrecioBase { get; }
    public int CapacidadMaxima { get; }
    public int CuposDisponibles { get; private set; }

    // Cada hijo declara cómo se llama y cómo explica su precio.
    public abstract string Tipo { get; }
    public abstract string DetallePrecio { get; }

    // CONSTRUCTOR: valida y deja el objeto en un estado correcto desde el inicio.
    protected Evento(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del evento no puede estar vacío.");
        if (precioBase < 0)
            throw new ArgumentException("El precio base no puede ser negativo.");
        if (capacidadMaxima <= 0)
            throw new ArgumentException("La capacidad máxima debe ser mayor que cero.");

        Id = id;
        Nombre = nombre.Trim();
        Fecha = fecha;
        PrecioBase = precioBase;
        CapacidadMaxima = capacidadMaxima;
        CuposDisponibles = capacidadMaxima; // al crearse, todos los cupos están libres
    }

    /// <summary>
    /// POLIMORFISMO: cada tipo de evento implementa su propia fórmula.
    /// </summary>
    public abstract decimal CalcularPrecioFinal();

    /// <summary>Descuenta cupos. Falla si la cantidad es inválida o excede los disponibles.</summary>
    public void ReservarCupos(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad de entradas debe ser mayor que cero.");
        if (cantidad > CuposDisponibles)
            throw new InvalidOperationException(
                $"No hay cupos suficientes. Solicitados: {cantidad}, disponibles: {CuposDisponibles}.");

        CuposDisponibles -= cantidad;
    }

    /// <summary>Devuelve cupos (cuando se cancela una reserva).</summary>
    public void DevolverCupos(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad a devolver debe ser mayor que cero.");
        if (CuposDisponibles + cantidad > CapacidadMaxima)
            throw new InvalidOperationException("No se pueden devolver más cupos que la capacidad máxima.");

        CuposDisponibles += cantidad;
    }
}
