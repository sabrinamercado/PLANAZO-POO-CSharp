using Planazo.Modelos;
using Planazo.Pruebas;
using Planazo.Servicios;
using Planazo.Utilidades;

namespace Planazo.Interfaz;


public class MenuConsola
{
    private readonly GestorReservas _gestor;

    public MenuConsola(GestorReservas gestor)
    {
        _gestor = gestor ?? throw new ArgumentNullException(nameof(gestor));
    }

    public void Ejecutar()
    {
        bool salir = false;

        while (!salir)
        {
            MostrarMenu();
            string? opcion = Console.ReadLine()?.Trim();
            if (opcion is null) break; // entrada cerrada: terminamos sin error
            Console.WriteLine();

            try
            {
                salir = ProcesarOpcion(opcion);
            }
            // EXCEPCIONES: cada error se atrapa y se muestra; el programa NO se cierra.
            catch (FormatException ex)
            {
                Console.WriteLine($"[!] Entrada incorrecta: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[!] Datos inválidos: {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[!] No encontrado: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[!] Operación no permitida: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error inesperado: {ex.Message}");
            }

            if (!salir) Pausar();
        }
    }

    private static void MostrarMenu()
    {
        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine("        PLANAZO - EVENTOS");
        Console.WriteLine("================================");
        Console.WriteLine();
        Console.WriteLine("1. Ver eventos disponibles");
        Console.WriteLine("2. Consultar evento");
        Console.WriteLine("3. Reservar entradas");
        Console.WriteLine("4. Ver reservas realizadas");
        Console.WriteLine("5. Cancelar una reserva");
        Console.WriteLine("6. Ver resumen de reservas");
        Console.WriteLine("7. Ejecutar casos de prueba automáticos (extra)");
        Console.WriteLine("0. Salir");
        Console.WriteLine();
        Console.Write("Seleccione una opción: ");
    }

    /// <returns>true si el usuario quiere salir.</returns>
    private bool ProcesarOpcion(string opcion)
    {
        switch (opcion)
        {
            case "1": VerEventos(); break;
            case "2": ConsultarEvento(); break;
            case "3": ReservarEntradas(); break;
            case "4": VerReservas(); break;
            case "5": CancelarReserva(); break;
            case "6": VerResumen(); break;
            case "7": CasosDePrueba.Ejecutar(); break;
            case "0":
                Console.WriteLine("¡Gracias por usar PLANAZO! Hasta pronto.");
                return true;
            default:
                Console.WriteLine("[!] Opción no válida. Elige un número del menú.");
                break;
        }
        return false;
    }

    // ---------- Opciones del menú ----------

    private void VerEventos()
    {
        Console.WriteLine("EVENTOS DISPONIBLES");
        Console.WriteLine();
        MostrarTabla(_gestor.Eventos);
    }

    private void ConsultarEvento()
    {
        MostrarTabla(_gestor.Eventos);
        Console.WriteLine();
        
        int id = LeerEntero("ID del evento: ");
        Evento e = _gestor.ObtenerEvento(id); // lanza KeyNotFoundException si no existe

        Console.WriteLine();
        Console.WriteLine($"Evento #{e.Id}: {e.Nombre}");
        Console.WriteLine($"  Tipo:               {e.Tipo}");
        Console.WriteLine($"  Fecha:              {Formato.Fecha(e.Fecha)}");
        Console.WriteLine($"  Precio base:        {Formato.Moneda(e.PrecioBase)}");
        Console.WriteLine($"  Ajuste de precio:   {e.DetallePrecio}");
        Console.WriteLine($"  Precio final:       {Formato.Moneda(e.CalcularPrecioFinal())}");
        Console.WriteLine($"  Cupos disponibles:  {e.CuposDisponibles} de {e.CapacidadMaxima}");
    }

    private void ReservarEntradas()
    {
        MostrarTabla(_gestor.Eventos);
        Console.WriteLine();

        int id = LeerEntero("ID del evento a reservar: ");
        Console.Write("Nombre de la persona: ");
        string nombre = Console.ReadLine() ?? "";
        int cantidad = LeerEntero("Cantidad de entradas: ");

        Reserva r = _gestor.Reservar(id, nombre, cantidad);

        Console.WriteLine();
        Console.WriteLine("[OK] Reserva confirmada.");
        Console.WriteLine($"  Reserva #{r.Id} a nombre de {r.NombrePersona}");
        Console.WriteLine($"  Evento: {r.Evento.Nombre}");
        Console.WriteLine($"  Entradas: {r.CantidadEntradas}  |  Total: {Formato.Moneda(r.ValorTotal)}");
        Console.WriteLine($"  Cupos restantes del evento: {r.Evento.CuposDisponibles}");
    }

    private void VerReservas()
    {
        if (_gestor.Reservas.Count == 0)
        {
            Console.WriteLine("Aún no hay reservas realizadas.");
            return;
        }

        Console.WriteLine("RESERVAS REALIZADAS");
        Console.WriteLine();
        foreach (Reserva r in _gestor.Reservas)
        {
            Console.WriteLine(
                $"#{r.Id} | {r.NombrePersona} | {r.Evento.Nombre} | " +
                $"{r.CantidadEntradas} entrada(s) | {Formato.Moneda(r.ValorTotal)} | {r.Estado}");
        }
    }

    private void CancelarReserva()
    {
        int id = LeerEntero("ID de la reserva a cancelar: ");
        Reserva r = _gestor.ObtenerReserva(id);
        int cuposAntes = r.Evento.CuposDisponibles;

        _gestor.CancelarReserva(id);

        Console.WriteLine();
        Console.WriteLine($"[OK] Reserva #{r.Id} cancelada.");
        Console.WriteLine($"  Cupos de \"{r.Evento.Nombre}\": {cuposAntes} -> {r.Evento.CuposDisponibles}");
    }

    private void VerResumen()
    {
        var (activas, entradas, total) = _gestor.ObtenerResumen();

        Console.WriteLine("RESUMEN DE RESERVAS ACTIVAS");
        Console.WriteLine();
        Console.WriteLine($"  Reservas activas:    {activas}");
        Console.WriteLine($"  Entradas reservadas: {entradas}");
        Console.WriteLine($"  Valor total:         {Formato.Moneda(total)}");
    }

    // ---------- Ayudas ----------

    private static void MostrarTabla(IEnumerable<Evento> eventos)
    {
        Console.WriteLine($"{"ID",-4}{"Tipo",-11}{"Nombre",-36}{"Fecha",-18}{"Precio final",-14}{"Cupos",-8}");
        Console.WriteLine(new string('-', 91));
        foreach (Evento e in eventos)
        {
            string cupos = $"{e.CuposDisponibles}/{e.CapacidadMaxima}";
            Console.WriteLine(
                $"{e.Id,-4}{e.Tipo,-11}{e.Nombre,-36}{Formato.Fecha(e.Fecha),-18}" +
                $"{Formato.Moneda(e.CalcularPrecioFinal()),-14}{cupos,-8}");
        }
    }

    private static int LeerEntero(string mensaje)
    {
        Console.Write(mensaje);
        string? texto = Console.ReadLine();

        if (!int.TryParse(texto, out int valor))
            throw new FormatException("debes escribir un número entero válido.");

        return valor;
    }

    private static void Pausar()
    {
        Console.WriteLine();
        Console.Write("Presiona Enter para volver al menú...");
        Console.ReadLine();
    }
}
