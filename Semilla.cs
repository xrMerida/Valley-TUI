using System;

namespace Granja;

/// <summary>
/// Representa un tipo de semilla con sus atributos de cultivo,
/// incluyendo nombre, meses de crecimiento, precio e ingresos.
/// </summary>
public class Semilla
{
    /// <summary>
    /// Nombre del cultivo (ej. Trigo, Repollo, Tomate).
    /// </summary>
    public string Nombre { get; }

    /// <summary>
    /// Cantidad de meses requeridos para que el cultivo esté listo para cosechar.
    /// </summary>
    public int Meses { get; }

    /// <summary>
    /// Precio de compra por unidad de semilla.
    /// </summary>
    public decimal Precio { get; }

    /// <summary>
    /// Ingresos generados al cosechar una parcela de este cultivo.
    /// </summary>
    public decimal Ingresos { get; }

    /// <summary>
    /// Cantidad disponible de esta semilla en el inventario.
    /// </summary>
    public int Cantidad { get; private set; }

    /// <summary>
    /// Identificador único auto-generado para cada tipo de semilla.
    /// </summary>
    public string Id { get; }

    private static int s_SemillaId = 12345;

    /// <summary>
    /// Inicializa un nuevo tipo de semilla con los parámetros especificados.
    /// </summary>
    /// <param name="nombre">Nombre del cultivo. No puede ser nulo ni vacío.</param>
    /// <param name="meses">Meses necesarios para la cosecha. Debe ser mayor a 0.</param>
    /// <param name="precio">Precio por unidad. No puede ser negativo.</param>
    /// <param name="ingresos">Ingresos al cosechar. Debe ser mayor que <paramref name="precio"/>.</param>
    /// <exception cref="ArgumentException">Si <paramref name="nombre"/> es nulo o vacío.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Si <paramref name="meses"/> ≤ 0, <paramref name="precio"/> negativo,
    /// o <paramref name="ingresos"/> ≤ <paramref name="precio"/>.
    /// </exception>
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

    /// <summary>
    /// Agrega una cantidad de unidades al inventario de esta semilla.
    /// </summary>
    /// <param name="cantidad">Cantidad a agregar. Debe ser mayor a 0.</param>
    /// <exception cref="ArgumentOutOfRangeException">Si <paramref name="cantidad"/> ≤ 0.</exception>
    public void AgregarCantidad (int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidad), "Cantidad a agregar debe ser mayor a 0");

        // else
        Cantidad += cantidad;
    }

    /// <summary>
    /// Disminuye en una unidad la cantidad disponible.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si la cantidad actual es 0.</exception>
    public void DisminuirCantidad ()
    {
        if (Cantidad <= 0)
            throw new InvalidOperationException("La cantidad de semillas es 0");

        // else
        Cantidad--;
    }

    /// <summary>
    /// Establece la cantidad disponible a 0.
    /// </summary>
    public void EliminarCantidad ()
        { Cantidad = 0; }
}
