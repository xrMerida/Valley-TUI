using System;

namespace Granja;

/// <summary>
/// Proporciona la navegacion y visualizacion de la cuadricula de parcelas
/// en dos dimensiones. Permite desplazarse con las flechas del teclado
/// y resalta la parcela seleccionada.
/// </summary>
public class MenuParcelas(Parcela[,] parcelas)
{
    /// <summary>
    /// Indice de la columna actualmente seleccionada en la cuadricula.
    /// </summary>
    public int SeleccionX { get; private set; }

    /// <summary>
    /// Indice de la fila actualmente seleccionada en la cuadricula.
    /// </summary>
    public int SeleccionY { get; private set; }

    /// <summary>
    /// Referencia a la matriz de parcelas que se mostrara y navegara.
    /// </summary>
    public Parcela[,] Parcelas { get; } = parcelas;

    // Secuencia de escape que elimina la linea actual
    private const string LIMPIAR_LINEA = "\e[2K\r";
    // Guarda la posicion del cursor, avanza a la siguiente linea y restaura la posicion
    private const string BORRAR_LINEA = "\e[2K\eM\r";

    /// <summary>
    /// Muestra la cuadricula completa de parcelas sin resaltar ninguna.
    /// Las parcelas cosechables se muestran en verde,
    /// las ocupadas en amarillo y las libres en color predeterminado.
    /// </summary>
    public void Mostrar()
    {
        // Recorre las filas (GetLength(1)) y columnas (GetLength(0))
        for (int i = 0; i < Parcelas.GetLength(0); i++)
        {
            for (int j = 0; j < Parcelas.GetLength(1); j++)
            {
                Console.ResetColor();
                Parcela parcela = Parcelas[i, j];

                // Colorea segun el estado de la parcela
                if (parcela.EsCosechable())
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (parcela.Semilla != null)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                // Muestra un espacio vacio o las iniciales de la planta
                if (parcela.Semilla == null)
                    Console.Write(" [ -- ] ");
                else
                    Console.Write($" [ {parcela.Semilla.Nombre[..2].ToUpper()} ] ");
            }

            // Salto de linea al terminar cada fila
            Console.WriteLine();
        }
        Console.ResetColor();
    }

    /// <summary>
    /// Muestra la cuadricula de parcelas resaltando la parcela actualmente
    /// seleccionada con un formato distintivo y color verde.
    /// </summary>
    public void MostrarSeleccion()
    {
        for (int i = 0; i < Parcelas.GetLength(0); i++)
        {
            for (int j = 0; j < Parcelas.GetLength(1); j++)
            {
                Console.ResetColor();
                Parcela parcela = Parcelas[i, j];

                // Resalta la parcela seleccionada
                if (i == SeleccionY && j == SeleccionX)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (parcela.Semilla == null)
                        Console.Write("[  --  ]");
                    else
                        Console.Write($"[  {parcela.Semilla.Nombre[..2].ToUpper()}  ]");
                }
                // Muestra la semilla plantada (dos letras)
                else if (parcela.Semilla != null)
                {
                    Console.Write($" [ {parcela.Semilla.Nombre[..2].ToUpper()} ] ");
                }
                // Parcela vacia
                else
                {
                    Console.Write(" [ -- ] ");
                }
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }

    /// <summary>
    /// Lee la entrada del teclado y actualiza la posicion seleccionada
    /// en la cuadricula usando las flechas direccionales.
    /// </summary>
    /// <param name="confirmarSalida">
    /// Si es <c>true</c>, al presionar Q se solicitara confirmacion antes de salir.
    /// </param>
    /// <returns>
    /// <c>true</c> si el usuario confirmo la seleccion (Enter) o solicito salir;
    /// <c>false</c> si solo navego entre parcelas.
    /// </returns>
    public bool Leer(bool confirmarSalida)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"\n{LIMPIAR_LINEA}      \u2190\u2191\u2193\u2192 navegar    \u21B2 seleccionar    Q salir  ");
        Console.ResetColor();
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        switch (keyInfo.Key)
        {
            case ConsoleKey.Q:
                if (confirmarSalida)
                {
                    // Solicita confirmacion antes de salir
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write($"{LIMPIAR_LINEA}   :  Salir ? [Y/n]  ");
                    Console.ResetColor();

                    if (Console.ReadKey(true).Key is not(ConsoleKey.Y or ConsoleKey.Enter))
                        return false;
                }
                SeleccionX = -1;
                SeleccionY = -1;
                Console.Write(BORRAR_LINEA);
                return true;

            // Confirma la parcela seleccionada
            case ConsoleKey.Enter:
                return true;

            // Navega hacia abajo
            case ConsoleKey.DownArrow:
                if (SeleccionY < Parcelas.GetLength(0) - 1)
                    SeleccionY++;
                // Vuelve a la primera fila si esta al final
                else
                    SeleccionY = 0;
                return false;

            // Navega hacia arriba
            case ConsoleKey.UpArrow:
                if (SeleccionY > 0)
                    SeleccionY--;
                // Va a la ultima fila si esta al inicio
                else
                    SeleccionY = Parcelas.GetLength(0) - 1;
                return false;

            // Navega hacia la derecha
            case ConsoleKey.RightArrow:
                if (SeleccionX < Parcelas.GetLength(1) - 1)
                    SeleccionX++;
                // Vuelve a la primera columna si esta al final
                else
                    SeleccionX = 0;
                return false;

            // Navega hacia la izquierda
            case ConsoleKey.LeftArrow:
                if (SeleccionX > 0)
                    SeleccionX--;
                // Va a la ultima columna si esta al inicio
                else
                    SeleccionX = Parcelas.GetLength(1) - 1;
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
    /// <c>true</c> si el usuario confirmo la seleccion (Enter) o solicito salir;
    /// <c>false</c> si solo navego entre parcelas.
    /// </returns>
    public bool Leer()
    {
        return Leer(true);
    }
}
