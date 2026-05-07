using System;

namespace Granja;

public static class Program
{
    static void Main()
    {
        static void WriteError (string mensajeError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"\r\e[K :: {mensajeError}\eM\r\e[K");
            Console.ResetColor();
        }

        static void CleanError ()
            { Console.Write("\e[K"); }

        Granja granja;
        int mesesRestantes;
        ////////// MENU INICIAL //////////
        { // Las variables no se guardaran
            Cultivo[] semillas;
            float dineroInicial;
            int empleados;
            float sueldo;
            int columnas;
            int filas;

            while (true) {
                Console.Write("Ingrese el dinero inical: ");

                if (!float.TryParse(Console.ReadLine(), out dineroInicial)
                        && dineroInicial < 100)
                {
                    WriteError("El dinero inical debe ser mayor a 100");
                    continue;
                }

                CleanError();
                break;
            }

            while (true) {
                Console.Write("Ingrese el numero de empleados: ");

                if (!int.TryParse(Console.ReadLine(), out empleados)
                        && empleados < 1)
                {
                    WriteError("Debe haber almenos un empleado trabajando");
                    continue;
                }

                CleanError();
                break;
            }

            while (true) {
                Console.Write("Ingrese el sueldo de los empleados: ");

                if (!float.TryParse(Console.ReadLine(), out sueldo)
                        && sueldo < 0)
                {
                    WriteError("Los empleados deben tener un sueldo");
                    continue;
                }

                CleanError();
                break;
            }

            while (true) {
                Console.Write("Ingrese las columnas de su granja");

                if (!int.TryParse(Console.ReadLine(), out columnas)
                        && columnas < 0)
                {
                    WriteError("Debe haber almenos una columna");
                    continue;
                }

                CleanError();
                break;
            }

            while (true) {
                Console.Write("Ingrese las filas de su granja");

                if (!int.TryParse(Console.ReadLine(), out filas)
                        && filas < 0)
                {
                    WriteError("Debe haber almenos una fila");
                    continue;
                }

                CleanError();
                break;
            }

            while (true) {
                Console.Write("Ingrese los meses a simular");

                if (!int.TryParse(Console.ReadLine(), out mesesRestantes)
                        && mesesRestantes < 0)
                {
                    WriteError("Debe simular almenos un mes");
                    continue;
                }

                CleanError();
                break;
            }

            semillas =
            [
                /////////// nombre        meses  precio  ingresos
                new Cultivo("Trigo",      1,     100,    130),
                new Cultivo("Repollo",    2,     180,    280),
                new Cultivo("Tomate",     3,     250,    450),
                new Cultivo("Calabaza",   4,     220,    360),
                new Cultivo("Esparrago",  6,     500,    1000),
            ];

            granja = new Granja
            (
                dineroInicial,
                empleados,
                sueldo,
                filas,
                columnas,
                semillas
            );
        }
        ////////// MENU PRINCIPAL //////////

        ////////// RESUMEN FINAL //////////
    }

}
