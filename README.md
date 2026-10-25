PLANAZO-POO-CSharp
Aplicación de consola en C# y .NET para gestionar los eventos de PLANAZO (cine, conciertos y talleres): consultar eventos, reservar entradas, controlar cupos y cancelar reservas. Parcial de Programación Orientada a Objetos – Programación con C# y .NET.
Autor: (Sabrina Mercado, edición apoyada con IA Claude)
---
1. Funcionalidades implementadas
Ver los eventos disponibles con su precio final y cupos.
Consultar un evento por su ID.
Reservar entradas (valida cantidad y cupos).
Ver las reservas realizadas (con su estado: Activa / Cancelada).
Cancelar una reserva (devuelve los cupos; no permite cancelar dos veces).
Ver resumen: reservas activas, entradas reservadas y valor total.
Seis eventos precargados: 2 cines, 2 conciertos y 2 talleres.
Reto adicional: opción 7 del menú que ejecuta automáticamente los 7 casos de prueba del parcial.
Valores monetarios con `decimal` y formato de moneda colombiano.
2. Cómo ejecutar
Requisitos: .NET SDK 8.0 o superior.

git clone https://github.com/<tu-usuario>/PLANAZO-POO-CSharp.git
cd PLANAZO-POO-CSharp

Si tienes otra versión del SDK, cambia `net8.0` en `PLANAZO-POO-CSharp.csproj` por tu versión.
3. Estructura del proyecto

PLANAZO-POO-CSharp/
├── Program.cs                  -> punto de entrada (solo arma y arranca la app)
├── Modelos/
│   ├── Evento.cs               -> clase abstracta base
│   ├── Cine.cs                 -> hereda de Evento
│   ├── Concierto.cs            -> hereda de Evento
│   ├── Taller.cs               -> hereda de Evento
│   └── Reserva.cs              -> asocia persona + evento (y enum EstadoReserva)
├── Servicios/
│   └── GestorReservas.cs       -> reglas de negocio y listas (List<T>)
├── Interfaz/
│   └── MenuConsola.cs          -> menú y lectura de datos del usuario
├── Pruebas/
│   └── CasosDePrueba.cs        -> casos de prueba automáticos (reto adicional)
├── Utilidades/
│   └── Formato.cs              -> formato de dinero y fechas
├── Capturas/                   -> evidencias de las pruebas
├── README.md
└── .gitignore
```

## 4\. Clases y cómo se relacionan

|Clase|Responsabilidad|
|-|-|
|`Evento` (abstracta)|Datos comunes (Id, Nombre, Fecha, PrecioBase, CapacidadMaxima, CuposDisponibles), manejo de cupos y el método abstracto `CalcularPrecioFinal()`.|
|`Cine`|Hereda de `Evento`. Precio final = precio base.|
|`Concierto`|Hereda de `Evento`. Precio final = precio base × 1,15.|
|`Taller`|Hereda de `Evento`. Precio final = precio base × 0,90.|
|`Reserva`|Guarda quién reservó, qué evento, cuántas entradas, el valor total y el estado. Sabe cancelarse.|
|`GestorReservas`|Tiene la `List<Evento>` y la `List<Reserva>`; crea reservas, cancela y calcula el resumen.|
|`MenuConsola`|Interacción con el usuario; atrapa las excepciones para que el programa no se cierre.|
|`CasosDePrueba`|Ejecuta los 7 escenarios del parcial automáticamente.|

**Relaciones:**


Evento (abstracta)
  ▲    ▲    ▲
  │    │    └── Taller        (herencia)
  │    └─────── Concierto     (herencia)
  └──────────── Cine          (herencia)

Reserva ───────► Evento        (asociación: una reserva apunta a un evento)
GestorReservas ◆─► List<Evento>, List<Reserva>   (composición: el gestor las contiene)
MenuConsola ───► GestorReservas                  (el menú usa al gestor)
```
5. Dónde se aplica cada concepto de POO
Encapsulamiento: en `Evento` y `Reserva` las propiedades solo tienen `get` (o `private set`). Por ejemplo, `CuposDisponibles` solo se puede modificar desde dentro de `Evento`, con los métodos `ReservarCupos()` y `DevolverCupos()`, que validan antes de cambiar el valor. `GestorReservas` expone sus listas como `IReadOnlyList`, para que nadie las modifique desde afuera.
Abstracción: `Evento` es `abstract`; define qué debe tener y hacer todo evento (incluyendo el método abstracto `CalcularPrecioFinal()`), sin decir cómo se calcula el precio. No se puede hacer `new Evento(...)`.
Herencia: `Cine`, `Concierto` y `Taller` heredan de `Evento` (`: Evento`) y reutilizan sus campos y la lógica de cupos sin repetir código.
Polimorfismo: cada hijo sobrescribe `CalcularPrecioFinal()` con `override`. En `Reserva` se hace `evento.CalcularPrecioFinal()` sin saber qué tipo de evento es, y C# ejecuta la fórmula correcta.
Constructores: validan los datos y dejan cada objeto en un estado correcto desde el inicio (por ejemplo, `Reserva` rechaza cantidades ≤ 0).
Composición y asociaciones: `GestorReservas` contiene las listas; `Reserva` se asocia con un `Evento`.
Colecciones: `List<Evento>` y `List<Reserva>` en `GestorReservas`.
Excepciones: `ArgumentException`, `InvalidOperationException` y `KeyNotFoundException` se lanzan en las reglas de negocio y se capturan en `MenuConsola` con `try/catch`; las entradas no numéricas generan `FormatException`.
6. Reglas de reservas y dónde se cumplen
Regla	Dónde
No cantidades ≤ 0	Constructor de `Reserva` y `Evento.ReservarCupos()`
No exceder los cupos	`Evento.ReservarCupos()`
Disminuir cupos al confirmar	`GestorReservas.Reservar()` llama a `ReservarCupos()`
Devolver cupos al cancelar	`Reserva.Cancelar()` llama a `DevolverCupos()`
No cancelar dos veces	`Reserva.Cancelar()` revisa el estado
7. Casos de prueba y capturas
Puedes probarlos a mano desde el menú o con la opción 7 (automáticos).
#	Caso	Resultado esperado	Captura
1	Reserva válida	Se confirma y bajan los cupos	`Capturas/01\_reserva\_valida.png`
2	Más entradas que cupos	Error controlado	`Capturas/02\_exceso\_cupos.png`
3	Cantidad negativa	Error controlado	`Capturas/03\_cantidad\_negativa.png`
4	Cancelar y devolver cupos	Los cupos vuelven a subir	`Capturas/04\_cancelacion.png`
5	Cancelar dos veces	Error controlado	`Capturas/05\_doble\_cancelacion.png`
6	Evento inexistente	Error controlado	`Capturas/06\_evento\_inexistente.png`
7	Precio final por tipo	Cine 18.000→18.000, Concierto 50.000→57.500, Taller 60.000→54.000	`Capturas/07\_precios.png`
(Inserta aquí las imágenes, por ejemplo: `!\[Reserva válida](Capturas/01\_reserva\_valida.png)`)
---
8. Pregunta de sustentación
¿Por qué utilizaste una clase abstracta `Evento` en lugar de crear clases completamente independientes para Cine, Concierto y Taller? ¿Qué ventajas tiene esta decisión cuando necesitamos agregar un nuevo tipo de evento?
Usé una clase abstracta porque Cine, Concierto y Taller comparten casi todo: identificador, nombre, fecha, precio base, capacidad y cupos, además de la lógica para reservar y devolver cupos. Lo único que cambia es cómo se calcula el precio final. Con una clase abstracta escribo lo común una sola vez y obligo a cada tipo a definir solo lo que lo hace diferente.
Ejemplos de mi código:
Evito repetir código. Los métodos `ReservarCupos()` y `DevolverCupos()` están escritos una sola vez en `Evento`. Si fueran tres clases independientes, tendría esas mismas validaciones copiadas tres veces, y un error corregido en una podría quedar sin corregir en las otras.
Obligo a cada tipo a tener su precio. `Evento` declara `public abstract decimal CalcularPrecioFinal();`, y cada hijo la implementa:
`Cine`: `PrecioBase`
`Concierto`: `PrecioBase \* (1 + Recargo)`
`Taller`: `PrecioBase \* (1 - Descuento)`
Si un tipo nuevo olvida implementarla, el compilador da error.
Puedo tratar todos los eventos igual (polimorfismo). `GestorReservas` guarda un solo `List<Evento>` con los tres tipos mezclados, y `Reserva` calcula `evento.CalcularPrecioFinal() \* cantidad` sin saber el tipo concreto. Con clases independientes necesitaría tres listas distintas o muchos `if`/`switch` por tipo.
Ventajas al agregar un nuevo tipo de evento (por ejemplo, `Teatro` con recargo del 5 %):
```csharp
public class Teatro : Evento
{
    public Teatro(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
        : base(id, nombre, fecha, precioBase, capacidadMaxima) { }

    public override string Tipo => "Teatro";
    public override string DetallePrecio => "Recargo del 5 %";
    public override decimal CalcularPrecioFinal() => PrecioBase \* 1.05m;
}
```
Solo creo esta clase y listo: no tengo que modificar `Reserva`, `GestorReservas` ni el menú, porque ya trabajan con `Evento`. Esto cumple el principio abierto/cerrado: el sistema está abierto a extenderse, pero no hace falta modificar el código que ya funciona, lo que reduce el riesgo de introducir errores y lo hace más fácil de mantener
