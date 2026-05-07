using System;

namespace Granja;

public class Cultivo
{
    private readonly string Nombre;
    private readonly int Meses;
    private readonly float Precio;
    private readonly float Ingresos;

    public Cultivo (string nombre, int meses, float precio, float ingresos)
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
    }

    public string GetNombre ()
        { return Nombre; }

    public int GetMeses ()
        { return Meses; }

    public float GetPrecio ()
        { return Precio; }

    public float GetIngresos ()
        { return Ingresos; }
}
