namespace Planazo.Modelos;

public enum EstadoReserva
{
    Activa,
    Cancelada

public class Reserva
{
    public int Id { get; }
    public string NombrePersona { get; }
    public Evento Evento { get; }
    public int CantidadEntradas { get; }
    public decimal ValorTotal { get; }
    public EstadoReserva Estado { get; private set; }

    public Reserva(int id, string nombrePersona, Evento evento, int cantidadEntradas)
    {
        if (string.IsNullOrWhiteSpace(nombrePersona))
            throw new ArgumentException("El nombre de la persona no puede estar vacío.");
        if (evento is null)
            throw new ArgumentNullException(nameof(evento), "La reserva necesita un evento.");
        if (cantidadEntradas <= 0)
            throw new ArgumentException("La cantidad de entradas debe ser mayor que cero.");

        Id = id;
        NombrePersona = nombrePersona.Trim();
        Evento = evento;
        CantidadEntradas = cantidadEntradas;
        ValorTotal = evento.CalcularPrecioFinal() * cantidadEntradas;
        Estado = EstadoReserva.Activa;
    }

    /// <summary>Cancela la reserva y devuelve los cupos al evento. No se puede cancelar dos veces.</summary>
    public void Cancelar()
    {
        if (Estado == EstadoReserva.Cancelada)
            throw new InvalidOperationException($"La reserva #{Id} ya estaba cancelada.");

        Evento.DevolverCupos(CantidadEntradas);
        Estado = EstadoReserva.Cancelada;
    }
}
