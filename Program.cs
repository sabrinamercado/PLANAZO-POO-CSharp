using System.Text;
using Planazo.Interfaz;
using Planazo.Servicios;

// Program.cs solo "arma" la aplicación: crea el gestor, carga los datos y abre el menú.
// Toda la lógica vive en las demás clases.
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var gestor = new GestorReservas();
gestor.CargarEventosIniciales();

var menu = new MenuConsola(gestor);
menu.Ejecutar();
