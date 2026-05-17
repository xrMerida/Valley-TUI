using System;
using System.Drawing;
using System.Globalization;
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
    static decimal IngresosTotales;
    static decimal DineroDelMes;
    static decimal GastoMateriaPrima;
    static bool JuegoFinalizado;
    static Menu MenuPrincipal = new([]);

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
        DineroDelMes = CapitalInicial;

        // Bucle principal del juego
        while (true)
        {
            Console.Clear();

            // Detecta si el usuario presiono Q
            if (JuegoFinalizado)
            {
                MenuPrincipal.ColorMensajeEstado = ColorError;
                MenuPrincipal.MensajeEstado = "JUEGO FINALIZADO\n";
                if (Granja.CajaEsperada <= 0)
                    MenuPrincipal.MensajeEstado += "   -> NO TIENES MAS DINERO";
                // Se suma 1 porque se toma en cuenta el siguiente mes
                else if (Granja.MesesSimulados >= MesesTotal + 1)
                    MenuPrincipal.MensajeEstado += "   -> NO TE QUEDAN MESES";
                else if (MenuPrincipal.Seleccion == -1)
                    MenuPrincipal.MensajeEstado += "   -> INTERRUPCION RECIBIDA";
                else
                    MenuPrincipal.MensajeEstado += "   -> ERROR";
            }

            ///////////// ENCABEZADO ////////////////
                MenuPrincipal.LimpiarEncabezado();
                MenuPrincipal.AgregarEncabezado($"Caja del Mes: ${DineroDelMes}");
                MenuPrincipal.AgregarEncabezado($"Caja: ${Granja.Caja}");
                MenuPrincipal.AgregarEncabezado($"Costos: ${Granja.Costos}");

                if (Granja.MesesSimulados < MesesTotal)
                    MenuPrincipal.AgregarEncabezado($"Mes: {Granja.MesesSimulados} / {MesesTotal}", ColorExito);
                else
                    MenuPrincipal.AgregarEncabezado($"ULTIMO MES: {Granja.MesesSimulados} / {MesesTotal}", ColorAdvertencia);
                // Coloca el color de la Caja esperada
                ConsoleColor colorCajaEsperada;
                if (Granja.CajaEsperada <= 0)
                    colorCajaEsperada = ColorError;
                else if (Granja.CajaEsperada <= DineroDelMes)
                    colorCajaEsperada = ColorAdvertencia;
                else
                    colorCajaEsperada = ColorExito;
                // Muestra la Caja esperada con el color seleccionado
                MenuPrincipal.AgregarEncabezado($"Caja esperada: ${Granja.CajaEsperada}", colorCajaEsperada);
            ///////////// FIN ENCABEZADO ////////////////

            // Cuando el juego termina se muestra el estado final y se sale
            if (JuegoFinalizado)
            {
                MenuPrincipal.MostrarEncabezado();
                MostrarContinuar();
                break;
            }

            // Bucle que muestra el menu y espera una accion del usuario
            // Separacion por encabezado constante
            do
            {
                Console.Clear();
                MenuPrincipal.MostrarEncabezado();
                MenuPrincipal.MostrarOpciones();
            } while (!MenuPrincipal.Leer());

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

    /// <summary>
    /// Solicita al usuario los parametros iniciales de la granja
    /// (empleados, sueldo, capital, meses, filas, columnas)
    /// e inicializa el objeto <see cref="global::Granja.Granja"/>.
    /// </summary>
    static void InicializarGranja()
    {
        int empleados = 2;
        decimal sueldo = 50;
        CapitalInicial = 500;
        MesesTotal = 5;
        int filas = 3;
        int columnas = 4;

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
                menu.AgregarEncabezado($"Caja: {Granja.Caja}");
                menu.AgregarEncabezado($"Costos: {Granja.Costos}");
                menu.AgregarEncabezado($"Utilidad: {Granja.Utilidad}");
                menu.AgregarEncabezado($"Gasto total: {gastoTotal}", ColorAdvertencia);
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
                menu.AgregarEncabezado($"Precio: {tienda[menu.Seleccion].Precio}", color);
                menu.AgregarEncabezado($"Meses: {tienda[menu.Seleccion].Meses}", color);
            }
            ///////////// FIN ENCABEZADO ////////////////

            Console.Clear();
            menu.MostrarEncabezado();
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
                if (!PreguntarContinuar("Comprar articulo"))
                {

                    continue;
                }

                // else
                tienda[menu.Seleccion].AgregarCantidad(cantidad);
                Granja.ComprarSemilla(tienda[menu.Seleccion]);
                tienda[menu.Seleccion].Reiniciar();
                gastoTotal = tienda[menu.Seleccion].Precio * cantidad;
                break;
            }
            GastoMateriaPrima += gastoTotal;
        }
    }

    static void Sembrar()
    {
        MenuParcelas menuParcelas = new(Granja.Parcelas);
        Menu menu = new([]);
        while (true)
        {
            //////////////// VERIFICACIONES ////////////////
            if (Granja.ParcelasLibres == 0)
            {
                MenuPrincipal.MensajeEstado = "No tienes parcelas para sembrar";
                MenuPrincipal.ColorMensajeEstado = ColorError;
                return;
            }

            if (Granja.InventarioSemillas.Length == 0)
            {
                MenuPrincipal.MensajeEstado = "No tienes semillas para plantar";
                MenuPrincipal.ColorMensajeEstado = ColorError;
                return;
            }

            // Le asigna las nuevas opciones al menu
            menu = new(new string[Granja.InventarioSemillas.Length]);
            for (int i = 0; i < menu.Opciones.Length; i++)
            {
                menu.Opciones[i] = Granja.InventarioSemillas[i].Nombre;
            }
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
                menu.MostrarEncabezado();
                menu.MostrarOpciones();
            } while (!menu.Leer(false));

            if (menu.Seleccion == -1)
                break;

            semilla = Granja.InventarioSemillas[menu.Seleccion];
            while (true)
            {
                Parcela parcela = Granja.Parcelas[menuParcelas.SeleccionY, menuParcelas.SeleccionX];
                ///////////////// ENCABEZADO /////////////////
                menu.LimpiarEncabezado();
                menu.AgregarEncabezado($"Posicion X: {menuParcelas.SeleccionX}");
                menu.AgregarEncabezado($"Posicion Y: {menuParcelas.SeleccionY}");
                menu.AgregarEncabezado($"Semilla: {semilla.Nombre}");
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
                menuParcelas.MostrarSeleccion();

                // Revisa si ya no se tiene de la semilla seleccionada en el inventario
                if (semilla.Cantidad == 0)
                {
                    menu.MensajeEstado = $"Sin semillas de {semilla.Nombre}";
                    menu.ColorMensajeEstado = ColorAdvertencia;
                    Console.WriteLine();
                    MostrarContinuar();
                    break;
                }

                if (!menuParcelas.Leer(false))
                    continue;

                if (menuParcelas.SeleccionX == -1)
                    break;

                if (!parcela.EstaLibre)
                {
                    menu.MensajeEstado = "Parcela ocupada";
                    menu.ColorMensajeEstado = ColorError;
                    continue;
                }

                if (!PreguntarContinuar("Plantar semilla"))
                    continue;

                Granja.Sembrar(menu.Seleccion, menuParcelas.SeleccionY, menuParcelas.SeleccionX);
                menu.MensajeEstado = "Semilla plantada";
                menu.ColorMensajeEstado = ColorExito;

                // Si el arreglo de inventario cambia de tamaño se ha quedado sin semillas
                if (Granja.InventarioSemillas.Length != menu.Opciones.Length)
                    semilla.Reiniciar();
            }
        }
    }

    static void ConsultarParcelas()
    {
    }

    static void AvanzarMes()
    {
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
                   Ingresos Totales: ${IngresosTotales}
                   Mano de Obra: ${Granja.ManoDeObra}
                   Inventario en Proceso: ${Granja.InventarioEnProceso}
                   Materia Prima: ${GastoMateriaPrima}
                   Utilidad Final: ${CapitalInicial
                                     + IngresosTotales
                                     + Granja.InventarioEnProceso
                                     - Granja.ManoDeObra
                                     - GastoMateriaPrima}

                {MenuPrincipal.MensajeEstado}

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

    static void MostrarContinuar()
    {
        Console.ForegroundColor = ColorInfo;
        Console.Write($"{LIMPIAR_LINEA} :: Presione enter para continuar ");
        Console.ResetColor();
        // Espera a que el usuario presione enter
        while (Console.ReadKey(true).Key != ConsoleKey.Enter);
        Console.Write(LIMPIAR_LINEA);
    }

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
                case ConsoleKey.N:
                    Console.Write(BORRAR_LINEA + LIMPIAR_LINEA);
                    return false;
                // Ignora otras teclas
                default:
                    continue;
            }
        }
    }
}
