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
            throw new ArgumentException("Los empelados deben de recibir un sueldo", nameof(sueldo));

        // lanza IndexOutOfRangeException cuando los valores son menores a 0
        Parcelas = new Parcela[columnas, filas];
        Dinero = dinero;
        Empleados = empleados;
        Sueldo = sueldo;
        MesesSimulados = 0;
        Semillas = [];
    }

    public Semilla[] GetSemillas ()
        { return Semillas; }
    public void GuardarSemillas (Semilla nuevaSemilla)
    {
        // Verificar si ya poseia semillas de la nueva semilla
        if (Semillas.Length > 0)
        {
            foreach (var semilla in Semillas)
            {
                if (semilla.GetNombre() == nuevaSemilla.GetNombre())
                {
                    semilla.AgregarCantidad(nuevaSemilla.GetCantidad());
                    Dinero -= nuevaSemilla.GetPrecio() * nuevaSemilla.GetCantidad();
                    return;
                }
            }
        }

        // Variable temporal para guardar las semillas almacenadas
        Semilla[] tempSemillas = new Semilla[Semillas.Length + 1];
        tempSemillas[^1] = nuevaSemilla;

        Dinero -= nuevaSemilla.GetPrecio();
        Semillas = tempSemillas;
    }
    public bool Sembrar (Semilla semilla, int fila, int columna)
    {
        // Solo permitir sembrar si esta vacio
        if (Parcelas[columna, fila].GetSemilla() != null)
            return false;

        // else
        Parcelas[columna, fila].SetSemilla(semilla);
        return true;
    }

    public void AvanzarMes ()
    {
        foreach (var parcela in Parcelas)
        {
            if (parcela.GetSemilla() == null)
                continue;

            // Si no se puede crecer significa que esta lista para cosechar
            if (!parcela.Crecer())
                Dinero += parcela.Cosechar();
        }

        MesesSimulados++;
        Dinero -= Empleados * Sueldo;
    }

    public decimal GetCostosEsperados ()
        { return Empleados * Sueldo; }

    public int GetMesesSimulados ()
        { return MesesSimulados; }
    public decimal GetIngresosEsperados ()
    {
        decimal ingresosEsperados = Dinero;
        foreach (var parcela in Parcelas)
        {
            // Si no se puede crecer significa que esta lista para cosechar
            if (!parcela.PuedeCrecer())
                ingresosEsperados += parcela.GetIngresos();
        }

        // Calcular los costos
        ingresosEsperados -= GetCostosEsperados();

        return ingresosEsperados;
    }

    public decimal GetDinero ()
        { return Dinero; }

    public decimal GetUtilidad ()
        { return Dinero - GetCostosEsperados(); }

    public Parcela[,] GetParcelas ()
        { return Parcelas; }
}
