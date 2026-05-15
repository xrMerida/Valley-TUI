using System;
namespace Granja;

/// <summary>
/// Punto de entrada principal del programa de gestión de granja.
/// Configura la granja con los datos ingresados por el usuario.
/// </summary>
static class Program
{
    static Granja granja = null!;
    static decimal capitalInicial;
    static int mesesTotal;

    static void Main()
    {
        InicializarGranja();
    }

    /// <summary>
    /// Solicita al usuario los parámetros iniciales de la granja
    /// (empleados, sueldo, capital, meses, filas, columnas)
    /// e inicializa el objeto <see cref="Granja"/>.
    /// </summary>
    static void InicializarGranja()
    {
        int empleados;
        decimal sueldo;
        int filas;
        int columnas;

        while (true)
        {
            ReiniciarLinea();
            Console.Write("Ingrese la cantidad de empleados: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (!int.TryParse(Console.ReadLine(), out empleados))
            {
                MostrarErrorLine("Ingrese un numero entero");
                continue;
            }
            if (empleados <= 0)
            {
                MostrarErrorLine("Se necesita almenos un empleado");
                continue;
            }

            break;
        }

        while (true)
        {
            ReiniciarLinea();
            Console.Write("Ingrese el sueldo de los empleados: $");
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (!decimal.TryParse(Console.ReadLine(), out sueldo))
            {
                MostrarErrorLine("Ingrese un numero real");
                continue;
            }
            if (sueldo <= 0)
            {
                MostrarErrorLine("Sueldo minimo es $1");
                continue;
            }

            break;
        }

        while (true)
        {
            ReiniciarLinea();
            Console.Write($"Ingrese su capital inicial [${sueldo * empleados}]: $");
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (!decimal.TryParse(Console.ReadLine(), out capitalInicial))
            {
                MostrarErrorLine("Ingrese un numero real");
                continue;
            }
            if (capitalInicial < sueldo * empleados)
            {
                MostrarErrorLine("Costos superan el capital inicial");
                continue;
            }

            break;
        }

        while (true)
        {
            ReiniciarLinea();
            Console.Write("Ingrese los meses a simular: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (!int.TryParse(Console.ReadLine(), out mesesTotal))
            {
                MostrarErrorLine("Ingrese un numero entero");
                continue;
            }
            if (mesesTotal <= 0)
            {
                MostrarErrorLine("Debe simular almenos un mes");
                continue;
            }

            break;
        }

        while (true)
        {
            ReiniciarLinea();
            Console.Write("Ingrese las filas: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (!int.TryParse(Console.ReadLine(), out filas))
            {
                MostrarErrorLine("Ingrese un numero entero");
                continue;
            }
            if (filas <= 0 || filas > 10)
            {
                MostrarErrorLine("Deben haber entre 1 y 10 filas");
                continue;
            }

            break;
        }

        while (true)
        {
            ReiniciarLinea();
            Console.Write("Ingrese las columnas: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (!int.TryParse(Console.ReadLine(), out columnas))
            {
                MostrarErrorLine("Ingrese un numero entero");
                continue;
            }
            if (columnas <= 0 || columnas > 10)
            {
                MostrarErrorLine("Deben haber entre 1 y 10 columnas");
                continue;
            }

            break;
        }

        ReiniciarLinea();
        granja = new(empleados, sueldo, capitalInicial, filas, columnas);
    }

    /// <summary>
    /// Muestra un mensaje de error en color rojo sin cambiar la línea actual.
    /// </summary>
    /// <param name="mensajeError">Texto del error a mostrar.</param>
    static void MostrarErrorLine(string mensajeError)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"\r\e[K :: {mensajeError}\eM\r\e[K");
        Console.ResetColor();
    }

    /// <summary>
    /// Resetea el color de consola, regresa al inicio de la linea actual y
    /// elimina todo su contenido.
    /// </summary>
    static void ReiniciarLinea()
    {
        Console.ResetColor();
        Console.Write("\r\e[K");
    }
}
