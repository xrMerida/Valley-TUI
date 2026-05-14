using System;

namespace Granja;

public class Semilla
{
    public string Nombre { get; }
    public int Meses { get; }
    public decimal Precio { get; }
    public decimal Ingresos { get; }
    public int Cantidad { get; private set; }
    public string Id { get; }
    private static int s_SemillaId = 12345;

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
        Id = s_SemillaId.ToString();
        s_SemillaId++;
    }

    public void AgregarCantidad (int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidad), "Cantidad a agregar debe ser mayor a 0");

        // else
        Cantidad += cantidad;
    }
    public void DisminuirCantidad ()
    {
        if (Cantidad <= 0)
            throw new InvalidOperationException("La cantidad de semillas es 0");

        // else
        Cantidad--;
    }

    public void EliminarCantidad ()
        { Cantidad = 0; }
}
