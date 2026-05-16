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
    static decimal capitalInicial;
    static int mesesTotal;
    static decimal ingresosTotales;
    static decimal dineroDelMes;
    static decimal gastoMateriaPrima;
    static bool juegoFinalizado;
    static string? mensajeEstado;

    const string LIMPIAR_LINEA = "\e[2K\r";
    const ConsoleColor ColorExito = ConsoleColor.Green;
    const ConsoleColor ColorError = ConsoleColor.Red;
    const ConsoleColor ColorAdvertencia = ConsoleColor.Yellow;

    static void Main()
    {
        ////////// MENU INICIAL //////////
        InicializarGranja();

        ////////// MENU PRINCIPAL //////////
        Menu menu = new()
        {
            Opciones =
            [
                "Comprar Semillas",
                "Sembrar",
                "Consultar Parcelas",
                "Avanzar de Mes"
            ]
        };
        dineroDelMes = capitalInicial;

        // Bucle principal del juego
        while (true)
        {
            Console.Clear();

            // Detecta si el usuario presiono Q
            juegoFinalizado = juegoFinalizado || menu.Seleccion == -1;

            if (juegoFinalizado)
            {
                mensajeEstado = "JUEGO FINALIZADO\n";
                if (Granja.CajaEsperada <= 0)
                    mensajeEstado += "   -> NO TIENES MAS DINERO";
                // Se le suma porque se toma en cuenta el siguiente mes
                else if (Granja.MesesSimulados >= mesesTotal + 1)
                    mensajeEstado += "   -> NO TE QUEDAN MESES";
                else if (menu.Seleccion == -1)
                    mensajeEstado += "   -> INTERRUPCION RECIBIDA";
                else
                    mensajeEstado += "   -> ERROR";
            }

            ///////////// ENCABEZADO ////////////////
            menu.LimpiarEncabezado();
            menu.AgregarEncabezado($"Caja del Mes: ${dineroDelMes}");
            menu.AgregarEncabezado($"Caja: ${Granja.Caja}");
            menu.AgregarEncabezado($"Costos: ${Granja.Costos}");

            if (Granja.MesesSimulados < mesesTotal)
                menu.AgregarEncabezado($"Mes: {Granja.MesesSimulados} / {mesesTotal}", ColorExito);
            else
                menu.AgregarEncabezado($"ULTIMO MES: {Granja.MesesSimulados} / {mesesTotal}", ColorAdvertencia);

            // Color segun el saldo esperado (verde si es positivo, amarillo si baja, rojo si quiebra)
            ConsoleColor colorCajaEsperada;
            if (Granja.CajaEsperada <= 0)
                colorCajaEsperada = ColorError;
            else if (Granja.CajaEsperada <= dineroDelMes)
                colorCajaEsperada = ColorAdvertencia;
            else
                colorCajaEsperada = ColorExito;

            menu.AgregarEncabezado($"Caja esperada: ${Granja.CajaEsperada}", colorCajaEsperada);

            if (juegoFinalizado)
                menu.AgregarEncabezado(mensajeEstado ?? "-", ColorError);
            else
                menu.AgregarEncabezado(mensajeEstado ?? "-");
            ///////////// FIN ENCABEZADO ////////////////

            // Cuando el juego termina se muestra el estado final y se sale
            if (juegoFinalizado)
            {
                menu.MostrarEncabezado();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\n :: Presione una tecla para continuar");
                Console.ReadKey(true);
                Console.Write("\eM\r\e[2K\r");
                Console.ResetColor();
                break;
            }

            // Bucle que muestra el menu y espera una accion del usuario
            do
            {
                Console.Clear();
                menu.MostrarEncabezado();
                menu.MostrarOpciones();
            } while (!menu.Leer());

            // Limpia el mensaje de estado para la siguiente iteracion
            mensajeEstado = null;

            // Sale del bucle principal si el usuario lo solicito
            if (menu.Seleccion == -1)
                continue;

            // Ejecuta la accion seleccionada
            Console.Clear();
            switch (menu.Opciones[menu.Seleccion])
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
        int empleados;
        decimal sueldo;
        int filas;
        int columnas;

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
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
        Console.ResetColor();

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
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
        Console.ResetColor();

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
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
        Console.ResetColor();

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
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
        Console.ResetColor();

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
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
        Console.ResetColor();

        while (true)
        {
            Console.Write(LIMPIAR_LINEA);
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
        Console.ResetColor();

        Granja = new(empleados, sueldo, capitalInicial, filas, columnas);
    }

    static void ComprarSemillas()
    {
        // Pendiente: implementar compra de semillas
    }

    static void Sembrar()
    {
        // Pendiente: implementar siembra
    }

    static void ConsultarParcelas()
    {
        // Pendiente: implementar consulta
    }

    static void AvanzarMes()
    {
        // Pendiente: implementar avance de mes
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
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"""

                   Capital Inicial: ${capitalInicial}
                   Ingresos Totales: ${ingresosTotales}
                   Mano de Obra: ${Granja.ManoDeObra}
                   Inventario en Proceso: ${Granja.InventarioEnProceso}
                   Materia Prima: ${gastoMateriaPrima}
                   Utilidad Final: ${capitalInicial
                                     + ingresosTotales
                                     + Granja.InventarioEnProceso
                                     - Granja.ManoDeObra
                                     - gastoMateriaPrima}

                {mensajeEstado}

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
}
