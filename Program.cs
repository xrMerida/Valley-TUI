using System;

namespace Granja;

static class Program
{

    ////////// VARIABLES //////////
    private static Granja granja;
    private static int mesesRestantes;
    private static decimal dineroInicial;
    private static readonly Semilla[] Semillas =
        [
            //  nombre        meses  precio  ingresos
            new("Trigo",      1,     100,    130),
            new("Repollo",    2,     180,    280),
            new("Tomate",     3,     250,    450),
            new("Calabaza",   4,     220,    360),
            new("Esparrago",  6,     500,    1000),
        ];


    static void Main()
    {
        ////////// MENU INICIAL //////////
        InicializarGranja ();

        ////////// MENU PRINCIPAL //////////

        // Bucle para mostrar menu principal
        int seleccion = 0;
        while (mesesRestantes > 0 || granja.GetDinero() > 0)
        {
            Console.Clear();
            string[] opciones =
            [
                "Comprar Semillas",
                "Sembrar",
                "Consultar Parcelas",
                "Avanzar de Mes"
            ];
            Menu.WriteSeleccionY(opciones, seleccion);

            // ManejarMenuY Retorna TRUE si el usuario selecciona una opcion (enter)
            if (Menu.ManejarMenuY(opciones.Length, ref seleccion))
            {
                // ManejarMenuY retorna -1 cuando el usuario selecciona salir
                if (seleccion == -1) break;

                Console.Clear();
                switch (opciones[seleccion])
                {
                    case "Comprar Semillas":
                        ComprarSemillas();
                        break;

                    case "Sembrar":
                        Sembrar();
                        break;

                    case "Consultar Parcelas":
                        ConsultarParcelas();
                        break;

                    case "Avanzar de Mes":
                        AvanzarMes();
                        break;
                }
            }
        }
    }
    static void ComprarSemillas ()
    {
        // Se crea un arreglo de opciones para mostrar en pantalla
        // utiliza el nombre de la semillas declaradas anteriormente
        string[] opciones = new string[Semillas.Length];
        for (int i = 0; i < Semillas.Length; i++)
            opciones[i] = Semillas[i].GetNombre();

        // Bucle de menu para comprar semillas
        int seleccion = 0;
        while (true)
        {
            Semilla semillaComprada;
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"""
                    -----
                       Caja: {granja.GetDinero()}
                       Costos: {granja.GetCostosEsperados()}
                       Utilidad: {granja.GetUtilidad()}
                    -----
                    """);
            if (granja.GetSemillas().Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                foreach (var semilla in granja.GetSemillas())
                    Console.WriteLine($"   {semilla.GetNombre()}: {semilla.GetCantidad()}");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("-----");
            }

            // Si la utilidad es menor a 0 no muestra el menu
            if (granja.GetUtilidad() < 0)
            {
                WriteError("\nYa no tienes dinero");
                WriteContinue();
                return;
            }

            Console.ResetColor();
            Menu.WriteSeleccionY(opciones, seleccion);

            if (!Menu.ManejarMenuY(opciones.Length, ref seleccion))
                continue;

            // Si el usuario selecciona salir
            if (seleccion == -1) return;
            // Asignar la semilla seleccionada a una variable temporal
            semillaComprada= new(Semillas[seleccion].GetNombre(), Semillas[seleccion].GetMeses(),
                      Semillas[seleccion].GetPrecio(), Semillas[seleccion].GetIngresos());
            // Formatear
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n :: Ingrese Q para salir");

            // Se obtiene la cantidad maxima a comprar
            //////// (Utilidad + UtilidadEsperada) / Precio \\\\\\\\\
            decimal cantidadMaxima = granja.GetUtilidadEsperada();
                // Agregar si las semilla a comprar se puede cosechar al siguiente mes
                if (semillaComprada.GetMeses() == 1)
                    cantidadMaxima += semillaComprada.GetIngresos();
                // dividir entre el precio
                cantidadMaxima /= semillaComprada.GetPrecio();
                // Convertir a numero entero
                cantidadMaxima = (int)cantidadMaxima;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($" Ingrese la cantidad de {opciones[seleccion]} [{cantidadMaxima:00}]: ");
                Console.ResetColor();

                string seleccionStr = Console.ReadLine() ?? "";

                // Cuando el usuario ingresa q o Q reinicia 
                if (seleccionStr.Trim() is "q" or "Q")
                    break;

                // Manejar errores de formato y cantidad
                if (!int.TryParse(seleccionStr, out int cantidad))
                {
                    WriteError("Ingrese un numero");
                    continue;
                }

                if (cantidad == 0)
                    break;

                // Cuando la cantidad no se puede ingresar
                if (!semillaComprada.SetCantidad(cantidad))
                {
                    WriteError("La cantidad debe ser mayor a 0");
                    continue;
                }

                // Si la granaja no puede pagarlo mostrara un error
                if (cantidad > cantidadMaxima)
                {
                    WriteError("No tienes suficiente dinero");
                    continue;
                }

                granja.ComprarSemilla(semillaComprada);
                break;
            }
        }
    }

    static void Sembrar ()
    {
        // Lista de opciones de las semillas
        string[] opciones = new string[granja.GetSemillas().Length];

        // Colocar el nombre de las semillas en el arreglo de opciones
        for (int i = 0; i < opciones.Length; i++)
            opciones[i] = granja.GetSemillas()[i].GetNombre();

        if (opciones.Length == 0)
        {
            WriteError("\n No tienes semillas para plantar");
            WriteContinue();
            return;
        }

        // Seleccion sera el indice de la semilla que se plantara
        int seleccion = 0;
        while (true)
        {
            Console.Clear();
            Menu.WriteSeleccionY(opciones, seleccion);
            if (!Menu.ManejarMenuY(opciones.Length, ref seleccion))
                continue;

            // Sale cuando el usuario lo solicita
            if (seleccion == -1)
                return;

            int seleccionX = 0;
            int seleccionY = 0;
            while (true)
            {
                Semilla? plantacion = granja.GetParcelas()[seleccionX,seleccionY].GetSemilla();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"""
                        ------
                           Posicion X: {seleccionX}
                           Posicion Y: {seleccionY}
                        """);
                if (plantacion == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"""   
                               Meses: 
                               Libre
                            """);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"""   
                               Meses: {granja.GetParcelas()[seleccionX, seleccionY].GetMesesSimulados()} / {plantacion.GetMeses()}
                               Ocupado
                            """);
                }

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("-----");
                Console.ResetColor();

                // j seran las filas de la granaja
                for (int j = 0; j < granja.GetParcelas().GetLength(1); j++)
                {
                    // i seran las columnas de la granja
                    for (int i = 0; i < granja.GetParcelas().GetLength(0); i++)
                    {
                        Console.ResetColor();

                        plantacion = granja.GetParcelas()[i,j].GetSemilla();
                        // Cuando la parcela esta seleccionada mostrarla en verde
                        if (seleccionX == i && seleccionY == j)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("\e[22m");
                        }

                        // Imprimir plantacion vacia
                        if (plantacion == null)
                            Console.Write(" [ ----- ] ");
                        // Imprimir nombre de la plantacion si esta ocupada
                        else
                            Console.Write($" [ {plantacion.GetNombre()} ] ");
                    }
                    // Salto de linea por cada fila impresa
                    Console.WriteLine();
                }

                // Se le pasan los argumentos para manejar un menu bidimensional
                if (!Menu.ManejarMenuXY(granja.GetParcelas().GetLength(1),
                                   granja.GetParcelas().GetLength(0),
                                   ref seleccionY,
                                   ref seleccionX))
                    // Si el usuario NO presiona enter continuar
                    { continue; }

                if (seleccionY is -1)
                    break;

                if (granja.Sembrar(seleccion, seleccionX, seleccionY))
                {
                    WriteContinue();
                    return;
                }
            }
        }
    }

    static void ConsultarParcelas ()
    {

    }

    static void AvanzarMes ()
    {

    }

    static void InicializarGranja ()
    {
        int empleados;
        decimal sueldo;
        int columnas;
        int filas;

        while (true)
        {
            Console.Write("Ingrese el numero de empleados: ");

            if (!int.TryParse(Console.ReadLine(), out empleados)
                    || empleados < 1)
            {
                WriteError("Debe haber almenos un empleado trabajando");
                continue;
            }

            CleanLine();
            break;
        }

        while (true)
        {
            Console.Write("Ingrese el sueldo de los empleados: ");

            if (!decimal.TryParse(Console.ReadLine(), out sueldo)
                    || sueldo < 0)
            {
                WriteError("Los empleados deben tener un sueldo");
                continue;
            }

            CleanLine();
            break;
        }

        while (true)
        {
            decimal dineroRequerido = empleados * sueldo;
            if (dineroRequerido < 100) dineroRequerido = 100;
            Console.Write($"Ingrese el dinero inical [${dineroRequerido}]: $");

            if (!decimal.TryParse(Console.ReadLine(), out dineroInicial))
            {
                WriteError("Ingrese un decimal");
                continue;
            }

            if (dineroInicial < dineroRequerido)
            {
                WriteError("Utilidad minima no alcanzada");
                continue;
            }

            CleanLine();
            break;
        }

        while (true)
        {
            Console.Write("Ingrese las columnas de su granja: ");

            if (!int.TryParse(Console.ReadLine(), out columnas)
                    || columnas <= 0)
            {
                WriteError("Debe haber almenos una columna");
                continue;
            }

            CleanLine();
            break;
        }

        while (true)
        {
            Console.Write("Ingrese las filas de su granja: ");

            if (!int.TryParse(Console.ReadLine(), out filas)
                    || filas <= 0)
            {
                WriteError("Debe haber almenos una fila");
                continue;
            }

            CleanLine();
            break;
        }

        while (true)
        {
            Console.Write("Ingrese los meses a simular: ");

            if (!int.TryParse(Console.ReadLine(), out mesesRestantes)
                    || mesesRestantes < 0)
            {
                WriteError("Debe simular almenos un mes");
                continue;
            }

            CleanLine();
            break;
        }

        // Repite el proceso si el usuario cancela la operación
        if (!AskContinue())
            InicializarGranja();

        // else
        granja = new Granja
        (
            dineroInicial,
            empleados,
            sueldo,
            filas,
            columnas
        );
    }
    static void WriteError (string mensajeError)
    {
        // Permite mostrar un error en consola sin cambiar el cursor de linea
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"\r\e[K :: {mensajeError}\eM\r\e[K");
        Console.ResetColor();
    }

    static bool AskContinue ()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(" :: Desea continuar? [Y/n]: ");
        Console.ResetColor();

        return (Console.ReadLine() ?? "").Trim() is not ("N" or "n");
    }

    static void WriteContinue ()
    {
        // Permite mostrar un error en consola sin cambiar el cursor de linea
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("\r\f\f :: Presione enter para continuar");
        Console.ResetColor();
        Console.ReadLine();
        Console.Write("\eM\r\e[K");
        Console.ResetColor();
    }
    static void CleanLine ()
        { Console.Write("\e[K"); }
}
