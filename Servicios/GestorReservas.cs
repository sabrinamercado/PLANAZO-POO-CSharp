using Planazo.Modelos;

namespace Planazo.Servicios;

public class GestorReservas
{
    private readonly List<Evento> _eventos = new();
    private readonly List<Reserva> _reservas = new();
    private int _siguienteIdReserva = 1;
    public IReadOnlyList<Evento> Eventos => _eventos;
    public IReadOnlyList<Reserva> Reservas => _reservas;

    public void AgregarEvento(Evento evento)
    {
        if (evento is null)
            throw new ArgumentNullException(nameof(evento));
        if (_eventos.Any(e => e.Id == evento.Id))
            throw new InvalidOperationException($"Ya existe un evento con ID {evento.Id}.");

        _eventos.Add(evento);
    }

    public Evento ObtenerEvento(int idEvento)
    {
        var evento = _eventos.FirstOrDefault(e => e.Id == idEvento);
        if (evento is null)
            throw new KeyNotFoundException($"No existe un evento con ID {idEvento}.");
        return evento;
    }

    public Reserva Reservar(int idEvento, string nombrePersona, int cantidad)
    {
        var evento = ObtenerEvento(idEvento);

        // 1) Se crea la reserva (valida nombre y cantidad > 0).
        var reserva = new Reserva(_siguienteIdReserva, nombrePersona, evento, cantidad);
        // 2) Se descuentan los cupos (valida que alcancen). Si falla, no se guarda nada.
        evento.ReservarCupos(cantidad);
        // 3) Solo si todo salió bien se registra la reserva y se avanza el consecutivo.
        _reservas.Add(reserva);
        _siguienteIdReserva++;

        return reserva;
    }

    public Reserva ObtenerReserva(int idReserva)
    {
        var reserva = _reservas.FirstOrDefault(r => r.Id == idReserva);
        if (reserva is null)
            throw new KeyNotFoundException($"No existe una reserva con ID {idReserva}.");
        return reserva;
    }

    public void CancelarReserva(int idReserva)
    {
        ObtenerReserva(idReserva).Cancelar();
    }

 
    public (int ReservasActivas, int EntradasReservadas, decimal ValorTotal) ObtenerResumen()
    {
        var activas = _reservas.Where(r => r.Estado == EstadoReserva.Activa).ToList();
        return (activas.Count,
                activas.Sum(r => r.CantidadEntradas),
                activas.Sum(r => r.ValorTotal));
    }

    /// <summary>Carga los seis eventos iniciales (2 cines, 2 conciertos, 2 talleres)
    public void CargarEventosIniciales()
    {
        AgregarEvento(new Cine(1, "The Hunger Games", new DateTime(2026, 11, 14, 20, 0, 0), 18000m, 40));
        AgregarEvento(new Cine(2, "How to lose a guy in 10 days", new DateTime(2026, 11, 15, 15, 0, 0), 15000m, 60));
        AgregarEvento(new Concierto(3, "Morat - Ya Es Mañana World Tour", new DateTime(2026, 12, 5, 19, 30, 0), 50000m, 100));
        AgregarEvento(new Concierto(4, "BTS - ARIRANG", new DateTime(2026, 12, 12, 20, 0, 0), 40000m, 80));
        AgregarEvento(new Taller(5, "Taller de Fotografía", new DateTime(2026, 11, 21, 9, 0, 0), 60000m, 20));
        AgregarEvento(new Taller(6, "Taller de Preparación de Cocteles", new DateTime(2026, 11, 28, 10, 0, 0), 80000m, 15));
    }
}
