using System;

namespace Granja;

/// <summary>
/// Representa la granja y gestiona su estado financiero, inventario
/// de semillas, parcelas y la simulación de meses.
/// </summary>
public class Granja
{
    /// <summary>
    /// Dinero disponible actualmente en caja.
    /// </summary>
    public decimal Caja { get; private set; }

    /// <summary>
    /// Cantidad de empleados contratados.
    /// </summary>
    public int Empleados { get; }

    /// <summary>
    /// Sueldo mensual por empleado.
    /// </summary>
    public decimal Sueldo { get; }

    /// <summary>
    /// Matriz bidimensional de parcelas que componen la granja.
    /// </summary>
    public Parcela[,] Parcelas { get; }

    /// <summary>
    /// Arreglo de semillas disponibles en el inventario.
    /// </summary>
    public Semilla[] InventarioSemillas { get; private set; }

    /// <summary>
    /// Cantidad de meses que han sido simulados.
    /// </summary>
    public int MesesSimulados { get; private set; }

    /// <summary>
    /// Costo mensual total: producto entre empleados y sueldo.
    /// </summary>
    public decimal Costos
    {
        get
        {
            return Empleados * Sueldo;
        }
    }

    /// <summary>
    /// Total acumulado de salarios pagados durante la simulación.
    /// </summary>
    public decimal ManoDeObra
    {
        get
        {
            return Empleados * Sueldo * MesesSimulados;
        }
    }

    /// <summary>
    /// Cantidad de parcelas actualmente libres (sin siembra).
    /// </summary>
    public int ParcelasLibres
    {
        get
        {
            int parcelasLibres = 0;
            foreach (var parcela in Parcelas)
            {
                if (parcela.EstaLibre())
                    parcelasLibres++;
            }

            return parcelasLibres;
        }
    }

    /// <summary>
    /// Valor total de las siembras aún no cosechadas.
    /// </summary>
    public decimal InventarioEnProceso
    {
        get
        {
            decimal inventarioEnProceso = 0;
            foreach (var parcela in Parcelas)
            {
                if (parcela.Semilla != null)
                    inventarioEnProceso += parcela.Ingresos;
            }

            return inventarioEnProceso;
        }
    }

    /// <summary>
    /// Utilidad actual: caja menos costos mensuales.
    /// </summary>
    public decimal Utilidad
    {
        get
        {
            return Caja - Costos;
        }
    }

    /// <summary>
    /// Total de ingresos generados por cosechas listas en el mes actual.
    /// </summary>
    public decimal Ingresos
    {
        get
        {
            decimal ingresos = 0;
            foreach (var parcela in Parcelas)
            {
                // Salta las parcelas libres
                if (parcela.EstaLibre())
                    continue;

                // Saltar las parcelas que no se pueden cosechar aun
                if (!parcela.EsCosechable())
                    continue;

                // else
                ingresos += parcela.Ingresos;
            }

            return ingresos;
        }
    }

    /// <summary>
    /// Proyección financiera: utilidad más ingresos por cosechar.
    /// </summary>
    public decimal CajaEsperada
    {
        get
        {
            return Utilidad + Ingresos;
        }
    }

    /// <summary>
    /// Obtiene la parcela en la posición especificada.
    /// </summary>
    /// <param name="fila">Índice de fila.</param>
    /// <param name="columna">Índice de columna.</param>
    /// <returns>La parcela en la posición indicada.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Si <paramref name="fila"/> o <paramref name="columna"/> no
    /// se encuentran en el arreglo.
    /// </exception>
    public Parcela GetParcela (int fila, int columna)
    {
        return Parcelas[fila, columna];
    }

    /// <summary>
    /// Inicializa la granja con la configuración inicial del usuario.
    /// </summary>
    /// <param name="empleados">Cantidad de empleados. Debe ser mayor a 0.</param>
    /// <param name="sueldo">Sueldo mensual por empleado. Debe ser mayor a 0.</param>
    /// <param name="caja">Capital inicial. No puede ser negativo ni cero.</param>
    /// <param name="filas">Cantidad de filas de la cuadrícula.</param>
    /// <param name="columnas">Cantidad de columnas de la cuadrícula.</param>
    /// <exception cref="ArgumentException">
    /// Si <paramref name="caja"/> ≤ 0, <paramref name="empleados"/> ≤ 0, o <paramref name="sueldo"/> ≤ 0.
    /// </exception>
    public Granja (int empleados,
                   decimal sueldo,
                   decimal caja,
                   int filas,
                   int columnas)
    {
        if (caja <= 0)
            throw new ArgumentException("No se puede iniciar con una deuda", nameof(caja));

        if (empleados <= 0)
            throw new ArgumentException("Se debe tener almenos un empleado", nameof(empleados));

        if (sueldo <= 0)
            throw new ArgumentException("Los empleados deben de recibir un sueldo", nameof(sueldo));

        // lanza IndexOutOfRangeException cuando los valores son menores a 0
        Parcelas = new Parcela[filas, columnas];

        // Inicializa todas las parcelas y las coloca con semillas vacias (null)
        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
                Parcelas[i,j] = new();
        }

        Caja = caja;
        Empleados = empleados;
        Sueldo = sueldo;
        MesesSimulados = 0;
        InventarioSemillas = [];
    }

    /// <summary>
    /// Compra una semilla y la agrega al inventario. Si ya existe
    /// una semilla del mismo tipo, incrementa su cantidad.
    /// </summary>
    /// <param name="nuevaSemilla">Semilla a comprar. No puede ser <c>null</c>.</param>
    /// <exception cref="ArgumentException">
    /// Si <paramref name="nuevaSemilla"/> es <c>null</c> o su cantidad es ≤ 0.
    /// </exception>
    public void ComprarSemilla (Semilla nuevaSemilla)
    {
        if (nuevaSemilla == null)
            throw new ArgumentException("No se puede comprar una semilla inexistente", nameof(nuevaSemilla));

        if (nuevaSemilla.Cantidad <= 0)
            throw new ArgumentException("No se pueden comprar 0 semillas", nameof(nuevaSemilla.Cantidad));

        // else
        // Verificar si ya poseia semillas de la nueva semilla
        if (InventarioSemillas.Length > 0)
        {
            for (int i = 0; i < InventarioSemillas.Length; i++)
            {
                // Continua si el Id no encaja
                if (InventarioSemillas[i].Id != nuevaSemilla.Id)
                    continue;

                // else
                InventarioSemillas[i].AgregarCantidad(nuevaSemilla.Cantidad);
                // retorna inmediatamente
                break;
            }
        }

        // Si no existia en el inventario
        else
        {
            //
            // Variable temporal para guardar las semillas almacenadas
            // Se le suma uno ya que necesita un nuevo espacio
            Semilla[] nuevoInventario = new Semilla[InventarioSemillas.Length + 1];
            // Se copia el arreglo de semillas al nuevo inventario
            for (int i = 0; i < InventarioSemillas.Length; i++)
                nuevoInventario[i] = InventarioSemillas[i];

            // Se coloca la nueva semilla al final
            nuevoInventario[^1] = nuevaSemilla;
            InventarioSemillas = nuevoInventario;
        }

        // Siempre se actualiza la cantidad de dinero
        Caja -= nuevaSemilla.Precio * nuevaSemilla.Cantidad;
    }

    /// <summary>
    /// Siembra una semilla del inventario en una parcela específica.
    /// </summary>
    /// <param name="indiceSemillas">Índice de la semilla en el inventario.</param>
    /// <param name="columna">Columna de la parcela destino.</param>
    /// <param name="fila">Fila de la parcela destino.</param>
    /// <exception cref="InvalidOperationException">
    /// Si la parcela destino ya está ocupada.
    /// </exception>
    public void Sembrar (int indiceSemillas, int columna, int fila)
    {
        // Adicionalmente lanza IndexOutOfRangeException
        if (Parcelas[columna, fila].Semilla != null)
            throw new InvalidOperationException("No se puede plantar en una parcela ocupada");

        // Adicionalmente lanza IndexOutOfRangeException
        Semilla semilla = InventarioSemillas[indiceSemillas];

        // else
        // Colocar la semilla en la parcela especificada
        Parcelas[columna, fila].Sembrar(semilla);

        // Si solo queda una semilla se elimina del inventario
        if (InventarioSemillas[indiceSemillas].Cantidad > 1)
        {
            InventarioSemillas[indiceSemillas].DisminuirCantidad();
        }
        // De lo contrario se disminuye la cantidad
        else
        {
            // Se le resta uno porque se eliminara una semilla
            Semilla[] nuevoInventario = new Semilla[InventarioSemillas.Length - 1];

            // j sera el contador del nuevo inventario
            int j = 0;
            // i sera el contador del inventario anterior
            for (int i = 0; i < InventarioSemillas.Length; i++)
            {
                // Saltar el indiceSemillas (no actualiza j)
                if (i == indiceSemillas)
                    continue;

                // else
                nuevoInventario[j] = InventarioSemillas[i];
                j++;
            }

            InventarioSemillas = nuevoInventario;
        }
    }

    /// <summary>
    /// Avanza un mes en la simulación: descuenta costos, hace crecer
    /// las siembras y cosecha las que están listas.
    /// </summary>
    public void AvanzarMes ()
    {
        Caja -= Costos;
        for (int i = 0; i < Parcelas.GetLength(0); i++)
        {
            for (int j = 0; j < Parcelas.GetLength(1); j++)
            {
                // Saltar las parcelas libres
                if (Parcelas[i,j].EstaLibre())
                    continue;

                // else
                if (Parcelas[i,j].EsCosechable())
                    Caja += Parcelas[i,j].Cosechar();
                else
                    Parcelas[i,j].Crecer();
            }
        }

        MesesSimulados++;
    }
}
