using System;

namespace Granja;

public class Semilla
{
    private readonly string Nombre;
    private readonly int Meses;
    private readonly decimal Precio;
    private readonly decimal Ingresos;
    private int Cantidad;

    public Semilla (string nombre,
                    int meses,
                    decimal precio,
                    decimal ingresos)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("Nombre es obligatorio", nameof(nombre));

        if (meses <= 0)
            throw new ArgumentOutOfRangeException(nameof(meses), "Meses debe ser mayor a 0");

        if (precio < 0)
            throw new ArgumentOutOfRangeException(nameof(precio), "Precio no puede ser negativo");

        if (ingresos <= precio)
            throw new ArgumentOutOfRangeException(nameof(precio), "Ingresos deben ser mayores al precio");

        Nombre = nombre;
        Meses = meses;
        Precio = precio;
        Ingresos = ingresos;
        Cantidad = 0;
    }

    public string GetNombre ()
        { return Nombre; }

    public int GetMeses ()
        { return Meses; }

    public decimal GetPrecio ()
        { return Precio; }

    public decimal GetIngresos ()
        { return Ingresos; }

    public int GetCantidad ()
        { return Cantidad; }

    public bool AgregarCantidad (int cantidad)
    {
        if (cantidad <= 0)
            { return false; }

        else
        {
            Cantidad += cantidad;
            return true;
        }
    }
    public bool DisminuirCantidad ()
    {
        if (Cantidad <= 1)
        {
            Cantidad = 0;
            return false;
        }

        else
        {
            Cantidad--;
            return true;
        }
    }
    public bool SetCantidad (int cantidad)
    {
        if (cantidad < 0)
            return false;

        // else
        Cantidad = cantidad;
        return true;
    }
}
