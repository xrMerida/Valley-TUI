using System;

namespace Granja;

/// <summary>
/// Gestiona la navegación y visualización de menús verticales en consola.
/// Administra su propio estado de selección y se encarga de construir
/// el encabezado linea por linea antes de mostrarlo.
/// </summary>
public class Menu
{
    /// <summary>
    /// Indice de la opcion actualmente seleccionada en el menu.
    /// </summary>
    public int Seleccion { get; set; }

    /// <summary>
    /// Arreglo de textos que se mostraran como opciones del menu.
    /// </summary>
    public string[] Opciones { get; set; }

    /// <summary>
    /// Mensaje dinamico que se muestra al final de cada menu.
    /// </summary>
    public string? MensajeEstado { get; private set; }

    /// <summary>
    /// Color del mesnaje dinamico que se muestra al final de cada menu.
    /// </summary>
    public ConsoleColor ColorMensajeEstado { get; private set; }

    /// <summary>
    /// Color que resalta la opcion seleccionada.
    /// </summary>
    public ConsoleColor ColorSeleccion { get; set; }

    /// <summary>
    /// Color principal de las lineas del encabezado si no se
    /// especifica un color
    /// </summary>
    public ConsoleColor ColorPrincipal { get; set; }

    /// <summary>
    /// Color secundario para los margenes del encabezado
    /// </summary>
    public ConsoleColor ColorSecundario { get; set; }
    /// <summary>
    /// Devuelve el texto de la opcion actualmente seleccionada.
    /// </summary>
    public string OpcionSeleccionada
    {
        get
        {
            return Opciones[Seleccion];
        }
    }

    // Almacena las lineas del encabezado y sus respectivos colores
    private string[] Encabezado { get; set; }
    private ConsoleColor[] ColoresEncabezado { get; set; }

    // Secuencia de escape que elimina el contendio de la linea actual
    // Se usa para actualizar el menu sin mover el cursor
    private const string LIMPIAR_LINEA = "\e[2K\r";
    // Guarda la posicion del cursor, avanza a la siguiente linea y restaura la posicion

    /// <summary>
    /// Crea un menu vacio con los valores predeterminados.
    /// </summary>
    public Menu(string[] opciones)
    {
        Seleccion = 0;
        Opciones = opciones;
        ColorSeleccion = ConsoleColor.Green;
        ColorPrincipal = ConsoleColor.Cyan;
        ColorSecundario = ConsoleColor.DarkGray;
        ColorMensajeEstado = ColorSecundario;
        MensajeEstado = null;
        Encabezado = [];
        ColoresEncabezado = [];
    }

    /// <summary>
    /// Agrega un mensaje con un color especifico al final del
    /// encabezado que se actualiza segun el estado del programa.
    /// </summary>
    /// <param name="mensaje">Mensaje de la linea a agregar.</param>
    /// <param name="color">Color con el que se mostrara el <paramref name="mensaje"/>.</param>
    public void SetMensajeEstado(string mensaje, ConsoleColor? color)
    {
        MensajeEstado = mensaje;
        ColorMensajeEstado = color ?? ColorSecundario;
    }

    /// <summary>
    /// Agrega un mensaje con un color el color secundario al final del
    /// encabezado que se actualiza segun el estado del programa.
    /// </summary>
    /// <param name="mensaje">Texto de la linea a agregar.</param>
    public void AgregarEncabezado(string mensaje)
    {
        AgregarEncabezado(mensaje, null);
    }

    /// <summary>
    /// Agrega una linea al encabezado del menu con un color especifico.
    /// </summary>
    /// <param name="texto">Texto de la linea a agregar.</param>
    /// <param name="color">Color con el que se mostrara la linea.</param>
    public void AgregarEncabezado(string texto, ConsoleColor? color)
    {
        color ??= ColorPrincipal;
        // Se copian los arreglos y luego se agrega el nuevo encabezado al final
        string[] nuevoEncabezado = new string[Encabezado.Length + 1];
        ConsoleColor[] nuevoColor = new ConsoleColor[ColoresEncabezado.Length + 1];

        for (int i = 0; i < Encabezado.Length; i++)
            nuevoEncabezado[i] = Encabezado[i];
        for (int i = 0; i < ColoresEncabezado.Length; i++)
            nuevoColor[i] = ColoresEncabezado[i];

        // Coloca el nuevo texto y color al final del arreglo
        nuevoEncabezado[^1] = texto;
        nuevoColor[^1] = (ConsoleColor)color;

        Encabezado = nuevoEncabezado;
        ColoresEncabezado = nuevoColor;
    }

    /// <summary>
    /// Agrega una linea al encabezado usando el color principal predeterminado.
    /// </summary>
    /// <param name="texto">Texto de la linea a agregar.</param>
    public void SetMensajeEstado(string texto)
    {
        AgregarEncabezado(texto, null);
    }

    /// <summary>
    /// Elimina todas las lineas del encabezado.
    /// </summary>
    public void LimpiarEncabezado()
    {
        Encabezado = [];
        ColoresEncabezado = [];
    }

    /// <summary>
    /// Muestra en consola todas las lineas del encabezado dentro de un marco.
    /// </summary>
    /// <param name="mostarMensajeEstado">
    /// Si es <c>true</c>, se muestra el contenido de MensajeEstado al final.
    /// </param>
    public void MostrarEncabezado(bool mostarMensajeEstado)
    {
        // Marco superior del encabezado
        Console.ForegroundColor = ColorSecundario;
        Console.WriteLine("------");

        // Imprime cada linea del encabezado con su color
        for (int i = 0; i < Encabezado.Length; i++)
        {
            Console.ForegroundColor = ColoresEncabezado[i];
            Console.WriteLine($"   {Encabezado[i]}");
        }

        if (mostarMensajeEstado)
        {
            Console.ForegroundColor = ColorMensajeEstado;
            Console.WriteLine($"   {MensajeEstado ?? "-"}");
            MensajeEstado = null;
            ColorMensajeEstado = ColorSecundario;
        }
        // Marco inferior del encabezado
        Console.ForegroundColor = ColorSecundario;
        Console.WriteLine("------");
        Console.ResetColor();
    }

    /// <summary>
    /// Muestra en consola todas las lineas del encabezado y el mensaje de estado
    /// dentro de un marco.
    /// </summary>
    public void MostrarEncabezado ()
    {
        MostrarEncabezado(true);
    }

    /// <summary>
    /// Muestra la lista de opciones numeradas resaltando la opcion seleccionada.
    /// </summary>
    public void MostrarOpciones()
    {
        for (int i = 0; i < Opciones.Length; i++)
        {
            Console.ResetColor();
            // Limpia la linea antes de escribir la opcion
            Console.Write(LIMPIAR_LINEA);

            // Muestra un indicador en la opcion seleccionada
            if (Seleccion == i)
            {
                Console.ForegroundColor = ColorSeleccion;
                Console.Write(" > ");
            }
            else
            {
                Console.Write("   ");
            }

            // Muestra el numero y el nombre de la opcion
            Console.WriteLine($"{i + 1}  {Opciones[i]}");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Lee la entrada del teclado y actualiza la seleccion del menu.
    /// </summary>
    /// <param name="confirmarSalida">
    /// Si es <c>true</c>, al presionar Q se solicitara confirmacion antes de salir.
    /// </param>
    /// <returns>
    /// <c>true</c> si el usuario confirmo una seleccion (Enter) o solicito salir;
    /// <c>false</c> si solo navego entre opciones.
    /// </returns>
    public bool Leer(bool confirmarSalida)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"{LIMPIAR_LINEA}      \u2191\u2193 navegar    \u21B5 seleccionar    Q salir  ");
        Console.ResetColor();

        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        // Permite seleccionar opciones directamente con los numeros del teclado
        if (char.IsDigit(keyInfo.KeyChar))
        {
            int digito = keyInfo.KeyChar - '0' - 1;
            if (digito >= 0 && digito < Opciones.Length)
                Seleccion = digito;

            return false;
        }

        switch (keyInfo.Key)
        {
            case ConsoleKey.Q:
                if (confirmarSalida)
                {
                    // Solicita confirmacion antes de salir
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write($"{LIMPIAR_LINEA}   :  Salir ? [Y/n]  ");
                    Console.ResetColor();

                    // Cancela la salida si no es Y
                    if (Console.ReadKey(true).Key is not (ConsoleKey.Y or ConsoleKey.Enter))
                        return false;
                }

                Seleccion = -1;
                Console.Write(LIMPIAR_LINEA);
                return true;

            // Confirma la opcion seleccionada
            case ConsoleKey.Enter:
                Console.Write(LIMPIAR_LINEA);
                return true;

            // Navega a la siguiente opcion 
            case ConsoleKey.DownArrow:
                if (Seleccion < Opciones.Length - 1)
                    Seleccion++;
                // Vuelve al inicio si esta al final
                else
                    Seleccion = 0;
                return false;

            // Navega a la opcion anterior
            case ConsoleKey.UpArrow:
                if (Seleccion > 0)
                    Seleccion--;
                // Va al final si esta al inicio
                else
                    Seleccion = Opciones.Length - 1;
                return false;

            // Ignora cualquier otra tecla
            default:
                return false;
        }
    }

    /// <summary>
    /// Lee la entrada del teclado con confirmacion de salida predeterminada.
    /// </summary>
    /// <returns>
    /// <c>true</c> si el usuario confirmo una seleccion (Enter) o solicito salir;
    /// <c>false</c> si solo navego entre opciones.
    /// </returns>
    public bool Leer()
    {
        return Leer(true);
    }
}
