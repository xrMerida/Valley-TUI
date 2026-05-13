using System;

namespace Granja;

static class Program
{

    ////////// VARIABLES //////////
    private static Granja granja = null!;
    private static int mesesTotal;
    private static string? mensajeEstado;
    private static ConsoleColor? colorEstado;
    private static decimal dineroInicial;
    private static decimal ingresosTotales;
    private static decimal dineroDelMes;
    private static decimal gastosTotales;
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
        int seleccion = 0;
        string[] opciones =
        [
            "Comprar Semillas",
            "Sembrar",
            "Consultar Parcelas",
            "Avanzar de Mes"
        ];

        // Bucle para mostrar menu principal
        // Hasta que se quede sin dinero o se acaben los meses
        while (true)
        {
            Console.Clear();

            ///////////// ENCABEZADO ////////////////
            bool juegoFinalizado = granja.GetMesesSimulados() >= mesesTotal || granja.GetDinero() <= 0 || seleccion == -1;
            if (juegoFinalizado)
            {
                ////////// SALIDA //////////
                mensajeEstado = "JUEGO FINALIZADO\n";
                if (granja.GetDinero() <= 0)
                    mensajeEstado += "   -> NO TIENES MAS DINERO";
                else if (granja.GetMesesSimulados() >= mesesTotal)
                    mensajeEstado += "   -> NO TE QUEDAN MESES";
                else
                    mensajeEstado += "   -> INTERRUPCION RECIBIDA";

                colorEstado = ConsoleColor.Red;
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"""
                    ------
                       Caja del Mes: ${dineroDelMes}
                       Caja: ${granja.GetDinero()}
                       Costos: ${granja.GetCostosEsperados()}
                    """);
            if (granja.GetMesesSimulados() < mesesTotal)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"   Mes: {granja.GetMesesSimulados()} / {mesesTotal}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (granja.GetMesesSimulados() >= mesesTotal || granja.GetDinero() <= 0)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"   UTLIMO MES: {granja.GetMesesSimulados()} / {mesesTotal}");
            }
            if (granja.GetSaldoEsperado() <= 0)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (granja.GetSaldoEsperado() <= dineroDelMes)
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            else if (granja.GetSaldoEsperado() > dineroDelMes)
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            if (granja.GetMesesSimulados() >= mesesTotal || granja.GetDinero() <= 0)
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"   Saldo esperado: ${granja.GetSaldoEsperado()}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("------");

            Console.ForegroundColor = colorEstado ?? ConsoleColor.DarkYellow;
            Console.WriteLine("   " + (mensajeEstado ?? "-"));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("------");
            Console.ResetColor();
            ///////////// FIN ENCABEZADO ////////////////
            if (juegoFinalizado)
            {
                Menu.WriteSeleccionY(opciones, -1);
                MostrarContinuar();
                break;
            }
            else
            {
                Menu.WriteSeleccionY(opciones, seleccion);
                mensajeEstado = null;
                colorEstado = null;
            }


            // ManejarMenuY Retorna TRUE si el usuario selecciona una opcion (enter)
            if (!Menu.ManejarMenuY(opciones.Length, ref seleccion, true))
                continue;

            // ManejarMenuY retorna -1 cuando el usuario selecciona salir
            // La salida se maneja al inicio del while
            if (seleccion == -1)
                continue;

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

        Console.Clear();
        Console.ResetColor();
        Console.WriteLine("""
                ██╗   ██╗ █████╗ ██╗     ██╗     ███████╗██╗   ██╗    ████████╗██╗   ██╗██╗
                ██║   ██║██╔══██╗██║     ██║     ██╔════╝╚██╗ ██╔╝    ╚══██╔══╝██║   ██║██║
                ██║   ██║███████║██║     ██║     █████╗   ╚████╔╝        ██║   ██║   ██║██║
                ╚██╗ ██╔╝██╔══██║██║     ██║     ██╔══╝    ╚██╔╝         ██║   ██║   ██║██║
                 ╚████╔╝ ██║  ██║███████╗███████╗███████╗   ██║          ██║   ╚██████╔╝██║
                  ╚═══╝  ╚═╝  ╚═╝╚══════╝╚══════╝╚══════╝   ╚═╝          ╚═╝    ╚═════╝ ╚═╝
                """);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"""
                   Capital Inicial: ${dineroInicial}
                   Ingresos Totales: ${ingresosTotales}
                   Inventario en Proceso: ${granja.GetIngresosEsperados()}
                   Mano de Obra: ${granja.GetManoDeObraTotal()}
                   Materia Prima: ${gastosTotales}
                   Utilidad Final: ${dineroInicial + granja.GetIngresosEsperados() - granja.GetManoDeObraTotal() - gastosTotales}

                Autor: Xavier Mérida
                """);
        Console.ResetColor();
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
        decimal gasto = 0;
        while (true)
        {
            // Asignar la semilla seleccionada a una variable temporal
            Semilla semillaSeleccion= new(Semillas[seleccion].GetNombre(), Semillas[seleccion].GetMeses(),
                      Semillas[seleccion].GetPrecio(), Semillas[seleccion].GetIngresos());
            // Se obtiene la cantidad maxima a comprar

            Console.Clear();
            ////////////// ENCABEZADO /////////////////
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"""
                    ------
                       Caja: ${granja.GetDinero()}
                       Costos: ${granja.GetCostosEsperados()}
                       Utilidad: ${granja.GetUtilidad()}
                    """);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"   Total Gastado: ${gasto}");
            if (granja.GetUtilidad() < 0
                    || granja.GetDinero() < semillaSeleccion.GetPrecio())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (granja.GetUtilidad() < 0)
                    Console.WriteLine("   Sin utilidad");
                else if (granja.GetDinero() < semillaSeleccion.GetPrecio())
                    Console.WriteLine("   Inasquible");
            }

            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   Compra Maxima: {Math.Round(granja.GetDinero() / semillaSeleccion.GetPrecio())}");
            }
            Console.WriteLine($"   Meses: {semillaSeleccion.GetMeses()}");
            Console.WriteLine($"   Precio: ${semillaSeleccion.GetPrecio()}");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("------");
            ////////////// FIN ENCABEZADO /////////////////

            // Muestra el inventario de semillas
            if (granja.GetSemillas().Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                // Obtiene todas las semillas y su cantidad del inventario de la granja
                foreach (var semilla in granja.GetSemillas())
                    Console.WriteLine($"   {semilla.GetNombre()}: {semilla.GetCantidad()}");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("-----");
            }
            Console.ResetColor();

            Menu.WriteSeleccionY(opciones, seleccion);

            if (!Menu.ManejarMenuY(opciones.Length, ref seleccion, false))
                continue;

            // Si el usuario selecciona salir
            if (seleccion == -1) break;

            // Si no se puede comprar
            if (granja.GetUtilidad() < 0
                    || granja.GetDinero() < semillaSeleccion.GetPrecio())
                { continue; }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n :: Ingrese Q para salir");


            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($" Ingrese la cantidad de {opciones[seleccion]}: ");
                Console.ResetColor();

                string seleccionStr = Console.ReadLine() ?? "";

                // Cuando el usuario ingresa q o Q reinicia 
                if (seleccionStr.Trim() is "q" or "Q")
                    break;

                // Manejar errores de formato y cantidad
                if (!int.TryParse(seleccionStr, out int cantidad))
                {
                    MostrarError("Ingrese un numero");
                    continue;
                }

                if (cantidad == 0)
                    break;

                // Cuando la cantidad no se puede ingresar
                if (!semillaSeleccion.SetCantidad(cantidad))
                {
                    MostrarError("La cantidad debe ser mayor a 0");
                    continue;
                }

                // Si la granaja no puede pagarlo mostrara un error
                if (granja.GetDinero() <= semillaSeleccion.GetPrecio() * cantidad)
                {
                    MostrarError("No tienes suficiente dinero");
                    continue;
                }

                granja.ComprarSemilla(semillaSeleccion);
                costo = semillaSeleccion.GetPrecio() * semillaSeleccion.GetCantidad();
                gasto += costo;
                gastosTotales += costo;
                break;
            }
        }
        mensajeEstado = $"Total Compra: ${gasto}";
    }

    static void Sembrar ()
    {
        // Seleccion sera el indice de la semilla que se plantara
        int seleccion = 0;
        while (true)
        {
            Console.Clear();

            // Lista de opciones de las semillas
            string[] opciones = new string[granja.GetSemillas().Length];

            // Colocar el nombre de las semillas en el arreglo de opciones
            for (int i = 0; i < opciones.Length; i++)
                opciones[i] = granja.GetSemillas()[i].GetNombre();

            if (opciones.Length == 0)
            {
                mensajeEstado = "No tienes semillas para plantar";
                colorEstado = ConsoleColor.Red;
                return;
            }

            if (granja.GetParcelasLibres() <= 0)
            {
                mensajeEstado = "No tienes parcelas donde plantar";
                colorEstado = ConsoleColor.Red;
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"""
                    ------
                       Inventario: {granja.GetSemillas()[seleccion].GetCantidad()}
                       Ingresos: ${granja.GetSemillas()[seleccion].GetIngresos()}
                       Parcelas Libres: {granja.GetParcelasLibres()}
                    ------
                    """);
            Console.ResetColor();
            Menu.WriteSeleccionY(opciones, seleccion);
            if (!Menu.ManejarMenuY(opciones.Length, ref seleccion, false))
                continue;

            // Sale cuando el usuario lo solicita
            if (seleccion == -1)
                return;

            int seleccionX = 0;
            int seleccionY = 0;
            bool? sembrado = null;
            int semillasSembradas = 0;
            string semillaNombre = granja.GetSemillas()[seleccion].GetNombre();
            while (true)
            {
                /////////// ENCABERZADO //////////
                Semilla? plantacion = granja.GetParcelas()[seleccionX,seleccionY].GetSemilla();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"""
                        ------
                           Posicion X: {seleccionX}
                           Posicion Y: {seleccionY}
                           Semilla: {semillaNombre}
                        """);
                try
                {
                    if (semillaNombre == granja.GetSemillas()[seleccion].GetNombre())
                        Console.WriteLine($"   Restantes: {granja.GetSemillas()[seleccion].GetCantidad()}");
                    else
                        throw new IndexOutOfRangeException();
                }
                catch (IndexOutOfRangeException)
                {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("""
                                   Restantes: 0
                                   -
                                   -
                                   -
                                """);
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("------");
                        MostrarParcelasSeleccion(-1, -1);
                        mensajeEstado = $"Semillas Sembradas: {semillasSembradas}";
                        MostrarContinuar();
                        seleccion = 0;
                        break;
                }
                if (sembrado == true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("   Plantación colocada");
                }
                else if (sembrado == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   No se pudo sembrar");
                }
                else if (sembrado == null)
                {

                    Console.WriteLine("   -");
                }
                sembrado = null;
                // Muestra los meses que le quedan a la parcela segun si estan vacios o no
                if (plantacion == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("""   
                               Meses: 
                               Libre
                            """);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    // Muestra los Meses que le quedan a la parcela
                    Console.WriteLine($"""   
                               Meses: {granja.GetParcelas()[seleccionX, seleccionY].GetMesesSimulados()} / {plantacion.GetMeses()}
                               Ocupado
                            """);
                }

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("-----");
                Console.ResetColor();
                /////////// FIN ENCABERZADO //////////

                MostrarParcelasSeleccion(seleccionX, seleccionY);

                // Se le pasan los argumentos para manejar un menu bidimensional
                if (!Menu.ManejarMenuXY(granja.GetParcelas().GetLength(1),
                                   granja.GetParcelas().GetLength(0),
                                   ref seleccionY,
                                   ref seleccionX))
                    // Si el usuario NO presiona enter continuar
                    { continue; }

                // Salir cuando el usuario selecciona la opcion de salir
                if (seleccionY is -1)
                    break;

                sembrado = granja.Sembrar(seleccion, seleccionX, seleccionY);
                if (sembrado == true) semillasSembradas++;
            }
        }
    }

    static void MostrarParcelasSeleccion (int seleccionX, int seleccionY)
    {
        // j seran las filas de la granaja
        for (int j = 0; j < granja.GetParcelas().GetLength(1); j++)
        {
            // i seran las columnas de la granja
            for (int i = 0; i < granja.GetParcelas().GetLength(0); i++)
            {
                Console.ResetColor();

                Semilla? plantacion = granja.GetParcelas()[i,j].GetSemilla();
                // Cuando la parcela esta seleccionada mostrarla en verde
                if (seleccionX == i && seleccionY == j)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    // Imprimir plantacion vacia
                    if (plantacion == null)
                        Console.Write("[  --  ]");
                    // Imprimir nombre de la plantacion si esta ocupada
                    // [..2] obtiene los primeros 2 caracteres del nombre de la semilla
                    else
                        Console.Write($"[  {plantacion.GetNombre()[..2].ToUpper()}  ]");
                }

                else
                {
                    // Coloca el texto en negrita
                    // Imprimir plantacion vacia
                    if (plantacion == null)
                        Console.Write(" [ -- ] ");
                    // Imprimir nombre de la plantacion si esta ocupada
                    // [..2] obtiene los primeros 2 caracteres del nombre de la semilla
                    else
                        Console.Write($" [ {plantacion.GetNombre()[..2].ToUpper()} ] ");
                }

            }
            // Salto de linea por cada fila impresa
            Console.WriteLine();
        }
    }

    static void ConsultarParcelas ()
    {
        int seleccionX = 0;
        int seleccionY = 0;
        do
        {
            Console.Clear();
            Parcela parcela = granja.GetParcelas()[seleccionX,seleccionY];

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"""
                    ------
                       Posicion X: {seleccionX}
                       Posicion Y: {seleccionY}
                    """);
            if (parcela.GetSemilla() == null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("""
                           Plantacion: Libre
                           Ingresos: $0
                           Meses: 0 / 0
                        """);
            }
            else if (parcela.GetSemilla() != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"""   
                           Plantacion: {parcela.GetSemilla()!.GetNombre()}
                           Ingresos: ${parcela.GetSemilla()!.GetIngresos()}
                           Meses: {parcela.GetMesesSimulados()} / {parcela.GetSemilla()!.GetMeses()}
                        """);
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("------");
            Console.ResetColor();


            MostrarParcelasSeleccion(seleccionX, seleccionY);

            // Se le pasan los argumentos para manejar un menu bidimensional
            Menu.ManejarMenuXY(granja.GetParcelas().GetLength(1), // El limite en Y
                               granja.GetParcelas().GetLength(0), // El limite en X
                               ref seleccionY,        // Seleccion del usuario en Y
                               ref seleccionX);       // Seleccion del usuario en X
        } while (seleccionY is not -1); // Si usuario sale del programa devuelve -1
    }

    static void MostrarParcelas ()
    {
        // j seran las filas 
        for (int j = 0; j < granja.GetParcelas().GetLength(1); j++)
        {
            // i seran las columnas de la granja
            for (int i = 0; i < granja.GetParcelas().GetLength(0); i++)
            {
                Console.ResetColor();

                // Variable de la parcela actual temporal
                Parcela parcela = granja.GetParcelas()[i,j];
                // Si es cosechable sera verde
                if (parcela.EsCosechable())
                    Console.ForegroundColor = ConsoleColor.Green;
                // Si no es cosechable y tiene semilla sera amarillo
                else if (parcela.GetSemilla() != null)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                // Si no tiene semilla sera blanco

                // Parcelas sin semilla
                if (parcela.GetSemilla() == null)
                    Console.Write(" [ -- ] ");
                // Parcelas cosechables seran resaltadas
                else if (parcela.EsCosechable())
                    Console.Write($" [ {parcela.GetSemilla()!.GetNombre()[..2].ToUpper()} ] ");
                // Parcelas no cosechables
                else
                    Console.Write($" [ {parcela.GetSemilla()!.GetNombre()[..2].ToUpper()} ] ");
            }
            // Salto de linea por cada fila impresa
            Console.WriteLine();
        }
    }
    static void AvanzarMes ()
    {
        ///////////// ENCABEZADO //////////////
        int parcelasCosechables = 0;
        foreach (var parcela in granja.GetParcelas())
        {
            if (parcela.EsCosechable())
                parcelasCosechables++;
        }
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"""
                ------
                   Parcelas a Cosechar: {parcelasCosechables}
                   Ingresos Esperados: {granja.GetIngresosEsperados()}
                """);
        if (granja.GetSaldoEsperado() <= 0)
            Console.ForegroundColor = ConsoleColor.Red;
        else if (granja.GetSaldoEsperado() <= dineroDelMes)
            Console.ForegroundColor = ConsoleColor.DarkYellow;
        else if (granja.GetSaldoEsperado() > dineroDelMes)
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"   Saldo esperado: ${granja.GetSaldoEsperado()}");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("------");
        Console.ResetColor();
        ///////////// FIN ENCABEZADO //////////////

        MostrarParcelas();
        Console.WriteLine();

        if (!AskContinuar())
        {
            mensajeEstado = "Operacion Cancelada";
            colorEstado = ConsoleColor.Red;
            return;
        }

        // else
        mensajeEstado = $"Ingresos: {granja.GetIngresosEsperados()}";
        ingresosTotales += granja.GetIngresosEsperados();
        granja.AvanzarMes();
        dineroDelMes = granja.GetDinero();
    }

    static void InicializarGranja ()
    {
        while (true)
        {
            int empleados;
            decimal sueldo;
            int columnas;
            int filas;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Inicalizar automaticamente");
                if (!AskContinuar())
                    break;
                // else

                // Randomizar todos los valores (segun restricciones)
                    Random random = new();
                    empleados = random.Next(1, 10);
                    sueldo = (decimal)Math.Round(random.Next(10, 501) * random.NextDouble(), 2);
                    dineroInicial = (decimal)Math.Round(random.Next(101, 1001) * random.NextDouble(), 2) + (sueldo * empleados) + 100;
                    if (sueldo * empleados < 101
                            && dineroInicial < 101)
                        { dineroInicial = 101; }
                    filas = random.Next(1, 11);
                    columnas = random.Next(1, 11);
                    mesesTotal = random.Next(1,15);
                    dineroDelMes = dineroInicial;

                granja = new Granja
                (
                    dineroInicial,
                    empleados,
                    sueldo,
                    filas,
                    columnas
                );

                // Mostrar los valores randomizados
                Console.WriteLine($"""
                        ------ COSTOS ------
                            Empleados: {empleados}
                            Sueldo: ${sueldo}
                            Total: ${granja.GetCostosEsperados()}
                        ------ Saldo -------
                            Inicial: ${dineroInicial}
                            Segundo Mes: ${granja.GetSaldoEsperado()}
                        ------ Extra -------
                            Meses Totales: {mesesTotal}
                            Filas: {filas}
                            Columnas: {columnas}
                        """);

                Console.WriteLine();
                MostrarParcelas();

                if (AskContinuar())
                    return;
            }

            while (true)
            {
                Console.Write("Ingrese el numero de empleados: ");

                if (!int.TryParse(Console.ReadLine(), out empleados)
                        || empleados < 1)
                {
                    MostrarError("Debe haber almenos un empleado trabajando");
                    continue;
                }

                if (empleados >= 10000)
                {
                    MostrarError("Ingrese una cantidad menor a 10000");
                    continue;
                }

                LimpiarLinea();
                break;
            }

            while (true)
            {
                Console.Write("Ingrese el sueldo de los empleados: ");

                if (!decimal.TryParse(Console.ReadLine(), out sueldo)
                        || sueldo < 0)
                {
                    MostrarError("Los empleados deben tener un sueldo");
                    continue;
                }

                if (sueldo >= 10000)
                {
                    MostrarError("Ingrese una cantidad menor a 10000");
                    continue;
                }

                LimpiarLinea();
                break;
            }

            while (true)
            {
                decimal dineroRequerido = (empleados * sueldo) + 1;
                // Siempre se solicita tener almenos 100
                if (dineroRequerido < 101) dineroRequerido = 101;
                Console.Write($"Ingrese el dinero inical [${dineroRequerido}]: $");

                if (!decimal.TryParse(Console.ReadLine(), out dineroInicial))
                {
                    MostrarError("Ingrese un decimal");
                    continue;
                }

                if (dineroInicial < dineroRequerido)
                {
                    MostrarError("Utilidad minima no alcanzada");
                    continue;
                }

                if (dineroInicial > 10000000)
                {
                    MostrarError("Ingrese una cantidad menor o igual a $100000000");
                    continue;
                }

                dineroDelMes = dineroInicial;
                LimpiarLinea();
                break;
            }

            while (true)
            {
                Console.Write("Ingrese las columnas de su granja: ");

                if (!int.TryParse(Console.ReadLine(), out columnas)
                        || columnas <= 0
                        || columnas > 10)
                {
                    MostrarError("Las cantidad de columnas debe estar entre 1 y 10");
                    continue;
                }

                LimpiarLinea();
                break;
            }

            while (true)
            {
                Console.Write("Ingrese las filas de su granja: ");

                if (!int.TryParse(Console.ReadLine(), out filas)
                        || filas <= 0
                        || filas > 10)
                {
                    MostrarError("Las cantidad de filas debe estar entre 1 y 10");
                    continue;
                }

                LimpiarLinea();
                break;
            }

            while (true)
            {
                Console.Write("Ingrese los meses a simular: ");

                if (!int.TryParse(Console.ReadLine(), out mesesTotal)
                        || mesesTotal < 0
                        || mesesTotal > 100)
                {
                    MostrarError("Solo se simularan entre 0 y 100 meses");
                    continue;
                }

                LimpiarLinea();
                break;
            }

            Console.WriteLine();
            MostrarParcelas();

            // Repite el proceso si el usuario cancela la operación
            if (!AskContinuar())
                continue;

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
    }
    static void MostrarError (string mensajeError)
    {
        // Permite mostrar un error en consola sin cambiar el cursor de linea
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"\r\e[K :: {mensajeError}\eM\r\e[K");
        Console.ResetColor();
    }

    static bool AskContinuar ()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(" :: Desea continuar? [Y/n]: ");
        Console.ResetColor();

        return (Console.ReadLine() ?? "").Trim() is not ("N" or "n" or "Q" or "q");
    }

    static void MostrarContinuar ()
    {
        // Permite mostrar un error en consola sin cambiar el cursor de linea
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("\r\f\f :: Presione una tecla para continuar");
        Console.ResetColor();
        Console.ReadKey(true);
        Console.Write("\eM\r\e[K");
        Console.ResetColor();
    }
    static void LimpiarLinea ()
        { Console.Write("\e[K"); }
}
