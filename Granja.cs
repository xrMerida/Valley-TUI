using System;

namespace Granja;

public class Granja
{
    public decimal Caja { get; private set; }
    public int Empleados { get; }
    public decimal Sueldo { get; }
    public Parcela[,] Parcelas { get; }
    public Semilla[] InventarioSemillas { get; private set; }
    public int MesesSimulados { get; private set; }
    public decimal Costos
    {
        get
        {
            return Empleados * Sueldo;
        }
    }

    public decimal ManoDeObra
    {
        get
        {
            return Empleados * Sueldo * MesesSimulados;
        }
    }

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

    public decimal Utilidad
    {
        get
        {
            return Caja - Costos;
        }
    }

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
    public decimal CajaEsperada
    {
        get
        {
            return Utilidad + Ingresos;
        }
    }

    public Granja (decimal dinero,
                   int empleados,
                   decimal sueldo,
                   int filas,
                   int columnas)
    {
        if (dinero < 0)
            throw new ArgumentException("No se puede iniciar con una deuda", nameof(dinero));

        if (empleados <= 0)
            throw new ArgumentException("Se debe tener almenos un empleado", nameof(empleados));

        if (sueldo < 0)
            throw new ArgumentException("Los empleados deben de recibir un sueldo", nameof(sueldo));

        // lanza IndexOutOfRangeException cuando los valores son menores a 0
        Parcelas = new Parcela[filas, columnas];

        // Inicializa todas las parcelas y las coloca con semillas vacias (null)
        for (int i = 0; i < columnas; i++)
        {
            for (int j = 0; j < filas; j++)
                Parcelas[i,j] = new();
        }

        Caja = dinero;
        Empleados = empleados;
        Sueldo = sueldo;
        MesesSimulados = 0;
        InventarioSemillas = [];
    }

    public Parcela GetParcela (int fila, int columna)
        { return Parcelas[fila, columna]; }

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
        if (InventarioSemillas[indiceSemillas].Cantidad <= 1)
            QuitarSemilla(indiceSemillas);
        // De lo contrario se disminuye la cantidad
        else
            InventarioSemillas[indiceSemillas].DisminuirCantidad();
    }

    private void QuitarSemilla (int indiceSemillas)
    {
        // Lanza IndexOutOfRangeException
        _ =InventarioSemillas[indiceSemillas];

        // else
        // Se le reta uno porque se eliminara una semilla
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
