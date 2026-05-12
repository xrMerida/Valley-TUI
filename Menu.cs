using System;

namespace Granja;
public static class Menu
{
    private static bool ManejarMenu (int limiteY,
                                     int limiteX,
                                     ref int seleccionY,
                                     ref int seleccionX,
                                     bool permitirDigitos,
                                     bool confirmarSalida)
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        // Obtiene cualquier numero ingresado incluyendo numpad
        if (permitirDigitos && char.IsDigit(keyInfo.KeyChar))
        {
            // Metodo convertir KeyChar a int devuelve el codigo ASCII
            // restar '0' lo convierte a el numero
            // retar 1 para obtener el indice del arreglo
            int digit = keyInfo.KeyChar - '0' - 1;
            // Solo se asignara a seleccion si la opcion existe
            if (digit >= 0 && digit < limiteY)
                seleccionY = digit;

            return false;
        }

        else
        {
            switch (keyInfo.Key)
            {
                // Usuario solicita salir
                case ConsoleKey.Q:
                    if (confirmarSalida)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("\r\e[K   :  Salir ? [Y/n]  ");
                        Console.ResetColor();

                        // Confirmación doble
                        keyInfo = Console.ReadKey(true);

                        if (keyInfo.Key is ConsoleKey.Y or ConsoleKey.Enter)
                        {
                            Console.Write("\e[2K\eM\r");
                            seleccionY = -1;
                            seleccionX = -1;
                            return true;
                        }
                        // else
                        return false;
                    }

                    else
                    {
                        seleccionX = -1;
                        seleccionY = -1;
                        return true;
                    }


                // Confirmar seleccion
                case ConsoleKey.Enter:
                    Console.Write("\e[2K\eM\r");
                    return true;

                // Siguiente opcion vertical
                case ConsoleKey.DownArrow:
                    // Retorna a la primer opcion si esta al final
                    if (seleccionY < limiteY - 1) seleccionY++;
                    else seleccionY = 0;
                    return false;

                // Opcion anterior vertical
                case ConsoleKey.UpArrow:
                    // Va a la ultima opcion si esta en el inicio
                    if (seleccionY > 0) seleccionY--;
                    else seleccionY = limiteY - 1;
                    return false;

                // Siguiente opcion horizontal
                case ConsoleKey.LeftArrow:
                    // Va a la ultima opcion si esta en el inicio
                    if (seleccionX > 0) seleccionX--;
                    else seleccionX = limiteX - 1;
                    return false;

                // Opcion anterior vertical
                case ConsoleKey.RightArrow:
                    // Retorna a la primer opcion si esta al final
                    if (seleccionX < limiteX - 1) seleccionX++;
                    else seleccionX = 0;
                    return false;

                // Entradas invalidas
                default:
                    return false;
            }
        }
    }

    public static bool ManejarMenuXY (int limiteY, int limiteX, ref int seleccionY, ref int seleccionX)
    {
        // Secuencia de escape '\e[1K' elimina la linea actual
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\e[K\n\e[K     ←↑↓→ navegar  ↲ seleccionar  Q salir");
        Console.ResetColor();

        return ManejarMenu(limiteY, limiteX, ref seleccionY, ref seleccionX, false, false);
    }
    public static bool ManejarMenuY (int limite, ref int seleccion, bool confirmarSalida)
    {
        // Secuencia de escape '\e[1K' elimina la linea actual
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\e[K\n\e[K     ↑↓ navegar  ↲ seleccionar  Q salir");
        Console.ResetColor();

        int _ = 0;

        return ManejarMenu(limite, _, ref seleccion, ref _, true, confirmarSalida);
    }

    public static void WriteSeleccionY (string[] opciones, int seleccion)
    {
        for (int i = 0; i < opciones.Length; i++)
        {
            // Elimina todo el contenido de la linea actual
            Console.Write("\e[K");

            // Muestra un ' > ' en verde junt con la opcion seleccionada
            if (seleccion == i)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" > ");
            }

            else
            {
                Console.Write("   ");
            }

            // Muestra el numero y el string de la opcion
            Console.Write(i + 1);
            Console.WriteLine($"  {opciones[i]}");
            Console.ResetColor();
        }
    }
}
