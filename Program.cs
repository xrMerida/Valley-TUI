using System;

namespace Granja;

/// <summary>
/// Punto de entrada principal del programa de gestion de granja.
/// Configura la granja, ejecuta el bucle principal del menu
/// y muestra el resumen final al terminar la simulacion.
/// </summary>
static class Program
{
    ////////// VARIABLES //////////
    static Granja Granja = null!;
    static decimal CapitalInicial;
    static int MesesTotal;
    static decimal CajaDelMes;
    static decimal GastoMateriaPrima;
    static bool JuegoFinalizado;
    static Menu MenuPrincipal = new([]);
    static MenuParcelas MenuParcelas = null!;
    const string LIMPIAR_LINEA = "\e[2K\r";
    const string BORRAR_LINEA = "\e[2K\eM\r";
    const ConsoleColor ColorExito = ConsoleColor.Green;
    const ConsoleColor ColorError = ConsoleColor.Red;
    const ConsoleColor ColorAdvertencia = ConsoleColor.Yellow;
    const ConsoleColor ColorInfo = ConsoleColor.Cyan;

    static void Main()
    {
        ////////// MENU INICIAL //////////
        InicializarGranja();

        ////////// MENU PRINCIPAL //////////
        MenuPrincipal =
        new([
            "Comprar Semillas",
            "Sembrar",
            "Consultar Parcelas",
            "Avanzar de Mes"
        ]);
        CajaDelMes = CapitalInicial;

        // Bucle principal del juego
        while (true)
        {
            // Coloca un mensaje en el encabezado cuando el juego termina
            if (JuegoFinalizado)
            {
                // Si el usuario sale del juego
                if (MenuPrincipal.Seleccion == -1)
                    MenuPrincipal.SetMensajeEstado("INTERRUPCION RECIBIDA", ColorAdvertencia);
                // Si el juego termino y la Caja esperada es menor a 0 entonces se quedo sin dinero
                else if (Granja.CajaEsperada <= 0)
                    MenuPrincipal.SetMensajeEstado("NO TIENES MÁS DINERO", ColorAdvertencia);
                // Si se acabaron los meses
                else if (Granja.MesesSimulados == MesesTotal)
                    MenuPrincipal.SetMensajeEstado("SE ACABARON LOS MESES", ColorExito);
                else
                    MenuPrincipal.SetMensajeEstado("ERROR", ColorError);
            }

            do
            {
                ///////////// ENCABEZADO ////////////////
                MenuPrincipal.LimpiarEncabezado();
                MenuPrincipal.AgregarEncabezado($"Caja del Mes: ${CajaDelMes}");
                MenuPrincipal.AgregarEncabezado($"Caja: ${Granja.Caja}");
                MenuPrincipal.AgregarEncabezado($"Costos: ${Granja.Costos}");
                // Muestra los meses restantes
                if (Granja.MesesSimulados < MesesTotal)
                    MenuPrincipal.AgregarEncabezado($"Mes: {Granja.MesesSimulados} / {MesesTotal}", ColorExito);
                else
                    MenuPrincipal.AgregarEncabezado($"ULTIMO MES: {Granja.MesesSimulados} / {MesesTotal}", ColorAdvertencia);
                // Coloca el color de la Caja esperada
                ConsoleColor colorCajaEsperada;
                if (Granja.CajaEsperada <= 0)
                    colorCajaEsperada = ColorError;
                else if (Granja.CajaEsperada <= CajaDelMes)
                    colorCajaEsperada = ColorAdvertencia;
                else
                    colorCajaEsperada = ColorExito;
                // Muestra la caja esperada con el color seleccionado
                MenuPrincipal.AgregarEncabezado($"Caja esperada: ${Granja.CajaEsperada}", colorCajaEsperada);
                ///////////// FIN ENCABEZADO ////////////////

                Console.Clear();
                MenuPrincipal.MostrarEncabezado();
                MenuPrincipal.MostrarOpciones();
                if (JuegoFinalizado)
                {
                    Console.WriteLine();
                    MostrarError("JUEGO TERMINADO");
                    MostrarContinuar();
                    break;
                }
            } while (!MenuPrincipal.Leer());

            if (JuegoFinalizado)
                break;

            // Seleccion es -1 cuando el usuario solicita salir
            if (MenuPrincipal.Seleccion == -1)
            {
                JuegoFinalizado = true;
                continue;
            }

            Console.Clear();
            switch (MenuPrincipal.Opciones[MenuPrincipal.Seleccion])
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

        ////////////// SALIDA //////////////
        MostrarResumenFinal();
    }

    static void InicializarGranja()
    {
        // Valores DEFAULT (plantilla)
        int empleados = 2;
        decimal sueldo = 45;
        CapitalInicial = 500;
        MesesTotal = 8;
        int filas = 2;
        int columnas = 3;

        if (PreguntarContinuar("Inicializar automaticamente (plantilla)"))
        {
            Granja = new(empleados, sueldo, CapitalInicial, filas, columnas);
            return;
        }

        // else
        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
            Console.Write("Ingrese la cantidad de empleados: ");
            Console.ForegroundColor = ColorInfo;
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
        Console.ResetColor();

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
            Console.Write("Ingrese el sueldo de los empleados: $");
            Console.ForegroundColor = ColorInfo;
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
        Console.ResetColor();

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
            Console.Write($"Ingrese su capital inicial [${sueldo * empleados}]: $");
            Console.ForegroundColor = ColorInfo;
            if (!decimal.TryParse(Console.ReadLine(), out CapitalInicial))
            {
                MostrarErrorLine("Ingrese un numero real");
                continue;
            }
            if (CapitalInicial < sueldo * empleados)
            {
                MostrarErrorLine("Costos superan el capital inicial");
                continue;
            }

            break;
        }
        Console.ResetColor();

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
            Console.Write("Ingrese los meses a simular: ");
            Console.ForegroundColor = ColorInfo;
            if (!int.TryParse(Console.ReadLine(), out MesesTotal))
            {
                MostrarErrorLine("Ingrese un numero entero");
                continue;
            }
            if (MesesTotal <= 0)
            {
                MostrarErrorLine("Debe simular almenos un mes");
                continue;
            }

            break;
        }
        Console.ResetColor();

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
            Console.Write("Ingrese las filas: ");
            Console.ForegroundColor = ColorInfo;
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
        Console.ResetColor();

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
            Console.Write("Ingrese las columnas: ");
            Console.ForegroundColor = ColorInfo;
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
        Console.ResetColor();

        Granja = new(empleados, sueldo, CapitalInicial, filas, columnas);
    }

    static void ComprarSemillas()
    {
        Semilla[] tienda =
        [
            //  nombre        meses  precio  ingresos
            new("Trigo",      1,     100,    130),
            new("Repollo",    2,     180,    280),
            new("Tomate",     3,     250,    450),
            new("Calabaza",   4,     220,    360),
            new("Esparrago",  6,     500,    1000),
        ];

        // Le asigna el mismo tamaño que la tienda
        Menu menu = new(new string[tienda.Length]);

        // Le asigna el nombre de cada semilla al menu
        for (int i = 0; i < tienda.Length; i++)
        {
            menu.Opciones[i] = tienda[i].Nombre;
        }

        decimal gastoTotal = 0;
        while (true)
        {
            bool esAsequible;
            ///////////// ENCABEZADO ////////////////
            {
                ConsoleColor color;
                menu.LimpiarEncabezado();
                menu.AgregarEncabezado($"Caja: ${Granja.Caja}");
                menu.AgregarEncabezado($"Costos: ${Granja.Costos}");
                menu.AgregarEncabezado($"Utilidad: ${Granja.Utilidad}");
                menu.AgregarEncabezado($"Gasto total: ${gastoTotal}", ColorAdvertencia);
                // Cuando ya no le queda utilidad
                if (Granja.Utilidad < 0)
                {
                    esAsequible = false;
                    color = ColorError;
                    menu.AgregarEncabezado("Sin utilidad", color);
                }
                // Cuando no tiene suficiente dinero
                else if (Granja.Caja < tienda[menu.Seleccion].Precio)
                {
                    color = ColorError;
                    menu.AgregarEncabezado("Inasequible", color);
                    esAsequible = false;
                }
                // Cuando lo puede comprar
                else
                {
                    color = ColorExito;
                    menu.AgregarEncabezado($"Compra Maxima: {Math.Floor(Granja.Caja / tienda[menu.Seleccion].Precio)}", color);
                    esAsequible = true;
                }
                menu.AgregarEncabezado($"Precio: ${tienda[menu.Seleccion].Precio}", color);
                menu.AgregarEncabezado($"Ingresos: ${tienda[menu.Seleccion].Ingresos}", color);
                menu.AgregarEncabezado($"Meses: {tienda[menu.Seleccion].Meses}", color);
            }
            ///////////// FIN ENCABEZADO ////////////////

            Console.Clear();
            menu.MostrarEncabezado(false);
            // Muestra el inventario
            if (Granja.InventarioSemillas.Length > 0)
            {
                Console.ForegroundColor = ColorAdvertencia;
                foreach (var semilla in Granja.InventarioSemillas)
                    Console.WriteLine($"   {semilla.Nombre}: {semilla.Cantidad}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("------");
            }
            menu.MostrarOpciones();

            if (!menu.Leer(false))
                continue;

            if (menu.Seleccion == -1)
                break;

            if (!esAsequible)
                continue;

            Console.ForegroundColor = ColorInfo;
            Console.WriteLine("\n :: Ingrese Q para regresar al menu");

            while (true)
            {
                Console.ResetColor();
                Console.Write($"Ingrese la cantidad de {menu.OpcionSeleccionada}: ");
                Console.ForegroundColor = ColorInfo;
                string seleccion = Console.ReadLine() ?? "";

                if (seleccion is "Q" or "q")
                    break;
                if (!int.TryParse(seleccion, out int cantidad))
                {
                    MostrarErrorLine("Ingrese un numero");
                    continue;
                }
                if (cantidad <= 0)
                {
                    MostrarErrorLine("La cantidad debe ser mayor a 0");
                    continue;
                }
                if (Granja.Caja < tienda[menu.Seleccion].Precio * cantidad)
                {
                    MostrarErrorLine("No tiene suficiente dinero");
                    continue;
                }

                // else
                tienda[menu.Seleccion].AgregarCantidad(cantidad);
                Granja.ComprarSemilla(tienda[menu.Seleccion]);
                tienda[menu.Seleccion].Reiniciar();
                gastoTotal = tienda[menu.Seleccion].Precio * cantidad;
                break;
            }
        }
        GastoMateriaPrima += gastoTotal;
        MenuPrincipal.SetMensajeEstado($"Total gastado: {gastoTotal}", ColorAdvertencia);
    }

    static void Sembrar()
    {
        Menu menu;
        while (true)
        {
            //////////////// VERIFICACIONES ////////////////
            if (Granja.ParcelasLibres == 0)
            {
                MenuPrincipal.SetMensajeEstado("No tienes parcelas para sembrar", ColorError);
                return;
            }

            if (Granja.InventarioSemillas.Length == 0)
            {
                MenuPrincipal.SetMensajeEstado("No tienes semillas para plantar", ColorError);
                return;
            }

            // Le asigna las nuevas opciones al menu
            menu = new(new string[Granja.InventarioSemillas.Length]);
            for (int i = 0; i < menu.Opciones.Length; i++)
                menu.Opciones[i] = Granja.InventarioSemillas[i].Nombre;
            //////////////// FIN VERIFICACIONES ////////////////


            Semilla semilla;
            do
            {
                semilla = Granja.InventarioSemillas[menu.Seleccion];
                ///////////////// ENCABEZADO /////////////////
                menu.LimpiarEncabezado();
                menu.AgregarEncabezado($"Parcelas Libres: {Granja.ParcelasLibres}");
                menu.AgregarEncabezado($"Ingresos: {semilla.Ingresos}", ColorExito);
                menu.AgregarEncabezado($"Inventario: {semilla.Cantidad}", ColorExito);
                ///////////////// FIN ENCABEZADO /////////////////

                Console.Clear();
                menu.MostrarEncabezado(false);
                menu.MostrarOpciones();
            } while (!menu.Leer(false));

            if (menu.Seleccion == -1)
                break;

            MenuParcelas = new (Granja.Parcelas);
            semilla = Granja.InventarioSemillas[menu.Seleccion];
            while (true)
            {
                Parcela parcela = Granja.Parcelas[MenuParcelas.SeleccionY, MenuParcelas.SeleccionX];
                ///////////////// ENCABEZADO /////////////////
                menu.LimpiarEncabezado();
                menu.AgregarEncabezado($"Posicion X: {MenuParcelas.SeleccionX}");
                menu.AgregarEncabezado($"Posicion Y: {MenuParcelas.SeleccionY}");
                menu.AgregarEncabezado($"Semilla: {semilla.Nombre}", ColorAdvertencia);
                // Mostrar si la parcela esta libre
                if (parcela.EstaLibre)
                    menu.AgregarEncabezado("Parcela: Libre", ColorExito);
                else
                    menu.AgregarEncabezado("Parcela: Ocupada", ColorAdvertencia);
                // Mostrar la cantidad restante
                if (semilla.Cantidad > 0)
                    menu.AgregarEncabezado($"Inventario: {semilla.Cantidad}", ColorExito);
                else
                    menu.AgregarEncabezado("Sin semillas", ColorError);
                ///////////////// FIN ENCABEZADO /////////////////

                Console.Clear();
                menu.MostrarEncabezado();
                MenuParcelas.MostrarSeleccion();

                // Revisa si ya no se tiene de la semilla seleccionada en el inventario
                if (semilla.Cantidad == 0)
                {
                    // Coloca el mensaje de estado para el menu anterior
                    menu.SetMensajeEstado($"Sin semillas de {semilla.Nombre}", ColorError);
                    Console.WriteLine();
                    MostrarContinuar();
                    break;
                }

                if (!MenuParcelas.Leer(false))
                    continue;

                if (MenuParcelas.SeleccionX == -1)
                    break;

                if (!parcela.EstaLibre)
                {
                    menu.SetMensajeEstado("Parcela ocupada", ColorError);
                    continue;
                }

                Granja.Sembrar(menu.Seleccion, MenuParcelas.SeleccionY, MenuParcelas.SeleccionX);
                menu.SetMensajeEstado("Semilla plantada", ColorExito);

                // Si el arreglo de inventario cambia de tamaño se ha quedado sin semillas
                if (Granja.InventarioSemillas.Length != menu.Opciones.Length)
                    semilla.Reiniciar();
            }
        }
    }

    /// <summary>
    /// Muestra el estado actual de todas las parcelas de la granja.
    /// </summary>
    static void ConsultarParcelas()
    {
        Menu menu = new([]);
        MenuParcelas = new (Granja.Parcelas);
        do
        {
            Parcela parcela = Granja.Parcelas[MenuParcelas.SeleccionY, MenuParcelas.SeleccionX];
            ///////////// ENCABEZADO ////////////////
            menu.LimpiarEncabezado();
            menu.AgregarEncabezado($"Posición X: {MenuParcelas.SeleccionX}");
            menu.AgregarEncabezado($"Posición Y: {MenuParcelas.SeleccionY}");
            if (parcela.Semilla == null)
            {
                menu.AgregarEncabezado("Plantacion: Libre", ColorAdvertencia);
                menu.AgregarEncabezado("Ingresos: $0", ColorAdvertencia);
                menu.AgregarEncabezado("Meses: 0 / 0", ColorAdvertencia);
            }
            else
            {
                menu.AgregarEncabezado($"Plantacion: {parcela.Semilla.Nombre}", ColorExito);
                menu.AgregarEncabezado($"Ingresos: {parcela.Ingresos}", ColorExito);
                menu.AgregarEncabezado($"Meses: {parcela.MesesSimulados} / {parcela.Semilla.Meses}", ColorExito);
            }
            ///////////// FIN ENCABEZADO ////////////////

            Console.Clear();
            menu.MostrarEncabezado(false);
            MenuParcelas.MostrarSeleccion();
            MenuParcelas.Leer(false);
        } while (MenuParcelas.SeleccionX != -1);
    }

    /// <summary>
    /// Avanza la simulacion un mes: descuenta costos, hace crecer
    /// las siembras, cosecha las parcelas listas y acumula ingresos.
    /// </summary>
    static void AvanzarMes()
    {
        // Asegura que el usuario plante todas las semillas posible antes de avanzar de mes
        if (Granja.InventarioSemillas.Length > 0 && Granja.ParcelasLibres > 0)
        {
            MenuPrincipal.SetMensajeEstado($"Aun tienes semillas y {Granja.ParcelasLibres} parcelas libres", ColorError);
            return;
        }

        Menu menu = new([]);
        MenuParcelas = new(Granja.Parcelas);
        int parcelasCosechables = 0;
        foreach (var parcela in Granja.Parcelas)
        {
            if (parcela.EsCosechable())
                parcelasCosechables++;
        }
        ///////////// ENCABEZADO ///////////////
        menu.LimpiarEncabezado();
        menu.AgregarEncabezado($"Parcelas a cosechar: {parcelasCosechables}");
        // Asigna el color de la los ingresos
        ConsoleColor colorIngresos;
        if (Granja.IngresosEsperados <= 0)
            colorIngresos = ColorError;
        else if (Granja.IngresosEsperados < Granja.Caja)
            colorIngresos = ColorAdvertencia;
        else
            colorIngresos = ColorExito;
        menu.AgregarEncabezado($"Ingresos Esperados: {Granja.IngresosEsperados}", colorIngresos);
        // Asigna el color de la caja esperada
        ConsoleColor colorCajaEsperada;
        if (Granja.CajaEsperada <= 0)
            colorCajaEsperada = ColorError;
        else if (Granja.CajaEsperada <= CajaDelMes)
            colorCajaEsperada = ColorAdvertencia;
        else
            colorCajaEsperada = ColorExito;
        menu.AgregarEncabezado($"Caja esperada: ${Granja.CajaEsperada}", colorCajaEsperada);
        ///////////// FIN ENCABEZADO ///////////////

        Console.Clear();
        menu.MostrarEncabezado(false);
        MenuParcelas.Mostrar();

        Console.WriteLine();
        if (!PreguntarContinuar("Avanzar de mes"))
        {
            MenuPrincipal.SetMensajeEstado("Operacion cancelada", ColorError);
            return;
        }

        // else
        MenuPrincipal.SetMensajeEstado($"Ingresos: {Granja.IngresosEsperados}", ColorExito);
        JuegoFinalizado = Granja.CajaEsperada <= 0;
        if (JuegoFinalizado)
            return;

        Granja.AvanzarMes();
        // Si es el ultimo mes finaliza el juego
        JuegoFinalizado = MesesTotal == Granja.MesesSimulados;
        CajaDelMes = Granja.Caja;
    }

    /// <summary>
    /// Muestra el resumen financiero al finalizar la simulacion.
    /// </summary>
    static void MostrarResumenFinal()
    {
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
        Console.ForegroundColor = ColorInfo;
        Console.WriteLine($"""

                   Capital Inicial: ${CapitalInicial}
                   Ingresos Totales: ${Granja.IngresosTotales}
                   Mano de Obra: ${Granja.ManoDeObra}
                   Inventario en Proceso: ${Granja.InventarioEnProceso}
                   Materia Prima: ${GastoMateriaPrima}
                   Utilidad Final: ${CapitalInicial
                                     + Granja.IngresosTotales
                                     + Granja.InventarioEnProceso
                                     - Granja.ManoDeObra
                                     - GastoMateriaPrima}

                Autor: Xavier Merida
                """);
        Console.ResetColor();
    }

    /// <summary>
    /// Muestra un mensaje de error en color rojo sin cambiar la linea actual.
    /// </summary>
    /// <param name="mensajeError">Texto del error a mostrar.</param>
    static void MostrarErrorLine(string mensajeError)
    {
        Console.ForegroundColor = ColorError;
        Console.Write($"{LIMPIAR_LINEA} :: {mensajeError}\eM{LIMPIAR_LINEA}");
        Console.ResetColor();
    }

    /// <summary>
    /// Muestra un mensaje de error en color rojo sin cambiar la linea actual.
    /// </summary>
    /// <param name="mensajeError">Texto del error a mostrar.</param>
    static void MostrarError(string mensajeError)
    {
        Console.ForegroundColor = ColorError;
        Console.WriteLine($"{LIMPIAR_LINEA} :: {mensajeError}");
        Console.ResetColor();
    }

    /// <summary>
    /// Muestra el mensaje "Presione enter para continuar" y espera
    /// a que el usuario presione la tecla Enter.
    /// </summary>
    static void MostrarContinuar()
    {
        Console.ForegroundColor = ColorInfo;
        Console.Write($"{LIMPIAR_LINEA} :: Presione enter para continuar");
        Console.ResetColor();
        // Espera a que el usuario presione enter
        while (Console.ReadKey(true).Key is not(ConsoleKey.Enter or ConsoleKey.Q));
        Console.Write(LIMPIAR_LINEA);
    }

    /// <summary>
    /// Muestra una pregunta de confirmacion y espera respuesta Y/N.
    /// </summary>
    /// <param name="pregunta">Texto de la pregunta a confirmar.</param>
    /// <returns><c>true</c> si el usuario confirma; <c>false</c> si niega.</returns>
    static bool PreguntarContinuar(string pregunta)
    {
        Console.ForegroundColor = ColorAdvertencia;
        Console.Write($"{LIMPIAR_LINEA} :: {pregunta}? [Y/n] ");
        Console.ResetColor();
        // Espera a que el usuario presione enter
        while (true)
        {
            switch (Console.ReadKey(true).Key)
            {
                // Confirmacion
                case ConsoleKey.Enter or ConsoleKey.Y:
                    Console.Write(BORRAR_LINEA);
                    return true;
                // Negacion
                case ConsoleKey.N or ConsoleKey.Q:
                    Console.Write(BORRAR_LINEA + LIMPIAR_LINEA);
                    return false;
                // Ignora otras teclas
                default:
                    continue;
            }
        }
    }
}
