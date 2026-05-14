using System;

namespace Granja;

public class Granja
{
    private decimal Dinero;
    private readonly int Empleados;
    private readonly decimal Sueldo;
    private readonly Parcela[,] Parcelas;
    private Semilla[] Semillas;
    private int MesesSimulados;

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
        Parcelas = new Parcela[columnas, filas];

        // Inicializa todas las parcelas y las coloca como vacias (null)
        for (int i = 0; i < columnas; i++)
        {
            for (int j = 0; j < filas; j++)
                Parcelas[i,j] = new();
        }

        Dinero = dinero;
        Empleados = empleados;
        Sueldo = sueldo;
        MesesSimulados = 0;
        Semillas = [];
    }

    public decimal GetManoDeObraTotal ()
        { return Empleados * Sueldo * MesesSimulados; }
    public Semilla[] GetSemillas ()
        { return Semillas; }
    public int GetCantidadSemillas ()
    {
        int cantidad = 0;
        foreach (var semilla in Semillas)
            cantidad += semilla.GetCantidad();

        return cantidad;
    }
    public int GetParcelasLibres ()
    {
        int parcelasLibres = 0;
        for (int i = 0; i < Parcelas.GetLength(0); i++)
        {
            for (int j = 0; j < Parcelas.GetLength(1); j++)
            {
                if (Parcelas[i,j].EstaLibre())
                    parcelasLibres++;
            }
        }

        return parcelasLibres;
    }
    public decimal GetInventarioEnProceso ()
    {
        decimal inventarioEnProceso = 0;
        for (int i = 0; i < Parcelas.GetLength(0); i++)
        {
            for (int j = 0; j < Parcelas.GetLength(1); j++)
            {
                if (Parcelas[i, j].GetSemilla() != null)
                    inventarioEnProceso += Parcelas[i, j].GetIngresos();
            }
        }

        return inventarioEnProceso;
    }
    public void ComprarSemilla (Semilla nuevaSemilla, int cantidad)
    {
        // Verificar si ya poseia semillas de la nueva semilla
        if (Semillas.Length > 0)
        {
            for (int i = 0; i < Semillas.Length; i++)
            {
                if (Semillas[i].GetNombre() == nuevaSemilla.GetNombre())
                {
                    Semillas[i].AgregarCantidad(cantidad);
                    Dinero -= nuevaSemilla.GetPrecio() * nuevaSemilla.GetCantidad();
                    return;
                }
            }
        }

        // Si la semilla no existia en el inventario

        // Variable temporal para guardar las semillas almacenadas
        // Se le suma uno ya que necesita un nuevo espacio
        Semilla[] tempSemillas = new Semilla[Semillas.Length + 1];
        for (int i = 0; i < Semillas.Length; i++)
            tempSemillas[i] = Semillas[i];

        // Se coloca la nueva semilla al final
        tempSemillas[^1] = nuevaSemilla;
        tempSemillas[^1].SetCantidad(cantidad);
        Semillas = tempSemillas;
        Dinero -= nuevaSemilla.GetPrecio() * cantidad;
    }
    public bool Sembrar (int indiceSemillas, int columna, int fila)
    {
        // Solo permitir sembrar si esta vacio
        if (Parcelas[columna, fila].GetSemilla() != null)
            return false;

        // Falso cuando el indice no existe
        if (indiceSemillas >= Semillas.Length
                || indiceSemillas < 0)
        { return false; }

        // else
        // Colocar la semilla en la parcela especificada
        Parcelas[columna, fila].SetSemilla(Semillas[indiceSemillas]);

        // Si solo queda una semilla (o menos) se elimina del inventario
        if (Semillas[indiceSemillas].GetCantidad() <= 1)
            QuitarSemilla(indiceSemillas);
        else
            Semillas[indiceSemillas].DisminuirCantidad();
        return true;
    }

    private bool QuitarSemilla (int indiceSemillas)
    {
        // Si el indice no existe
        if (indiceSemillas > Semillas.Length
            || indiceSemillas < 0)
        { return false; }

        // Se le reta uno porque se eliminara una semilla
        Semilla[] tempSemillas = new Semilla[Semillas.Length - 1];

        // j sera el contador de temp semillas
        int j = 0;
        // i sera el contador de Semillas
        for (int i = 0; i < Semillas.Length; i++)
        {
            // Saltar el indiceSemillas
            if (i == indiceSemillas)
                continue;

            // else
            tempSemillas[j] = Semillas[i];
            j++;
        }

        Semillas = tempSemillas;
        return true;
    }

    public void AvanzarMes ()
    {
        Dinero = GetUtilidad();
        for (int i = 0; i < Parcelas.GetLength(0); i++)
        {
            for (int j = 0; j < Parcelas.GetLength(1); j++)
            {
                if (Parcelas[i,j].EstaLibre())
                    continue;

                Dinero += Parcelas[i,j].CosecharYCrecer();
            }
        }

        MesesSimulados++;
    }

    public int GetMesesSimulados ()
        { return MesesSimulados; }
    public decimal GetIngresosEsperados ()
    {
        decimal ingresosEsperados = 0;
        for (int i = 0; i < Parcelas.GetLength(0); i++)
        {
            for (int j = 0; j < Parcelas.GetLength(1); j++)
            {
                if (Parcelas[i,j].EstaLibre())
                    continue;

                if (Parcelas[i,j].EsCosechable())
                    ingresosEsperados += Parcelas[i,j].GetIngresos();
            }
        }

        return ingresosEsperados;
    }
    public decimal GetCajaEsperada ()
        { return GetUtilidad() + GetIngresosEsperados(); }

    public decimal GetDinero ()
        { return Dinero; }

    public decimal GetCostosEsperados ()
        { return Empleados * Sueldo; }

    public decimal GetUtilidad ()
        { return Dinero - GetCostosEsperados(); }

    public Parcela[,] GetParcelas ()
        { return Parcelas; }
}
