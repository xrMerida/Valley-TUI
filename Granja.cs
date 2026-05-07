using System;

namespace Granja;

public class Granja
{
    private readonly float DineroInicial;
    private float Dinero;
    private readonly int Empleados;
    private readonly float Sueldo;
    private Parcela[,] Parcelas;
    private Cultivo[] Semillas;
    private int MesesSimulados;

    public Granja (float dinero,
                   int empleados,
                   float sueldo,
                   int filas,
                   int columnas,
                   Cultivo[] semillas)
    {
        if (dinero < 0)
            throw new ArgumentException("No se puede iniciar con una deuda", nameof(dinero));

        if (empleados <= 0)
            throw new ArgumentException("Se debe tener almenos un empleado", nameof(empleados));

        if (sueldo < 0)
            throw new ArgumentException("Los empelados deben de recibir un sueldo", nameof(sueldo));

        if (semillas.Length == 0)
            throw new ArgumentException("Se debe tener almenos una semilla", nameof(sueldo));

        // throw IndexOutOfRangeException cuando los valores son menores a 0
        Parcelas = new Parcela[columnas, filas];
        Dinero = dinero;
        DineroInicial = dinero;
        Empleados = empleados;
        Sueldo = sueldo;
        Semillas = semillas;
        MesesSimulados = 0;
    }

    public bool Sembrar (Cultivo semilla, int fila, int columna)
    {
        // Solo permitir sembrar si esta vacio
        if (Parcelas[columna, fila].GetSemilla() != null)
            return false;

        Parcelas[columna, fila].SetSemilla(semilla);
        return true;
    }

    public void AvanzarMes ()
    {
        foreach (Parcela parcela in Parcelas)
        {
            if (parcela.GetSemilla() == null)
                continue;

            // Si no se puede crecer significa que esta lista para cosechar
            if (!parcela.Crecer())
                Dinero += parcela.Cosechar();
        }

        Dinero -= Empleados * Sueldo;
    }

    public float IngresosEsperados ()
    {
        float ingresosEsperados = Dinero;
        foreach (Parcela parcela in Parcelas)
        {
            if (parcela.GetSemilla() == null)
                continue;

            // Si no se puede crecer significa que esta lista para cosechar
            if (!parcela.PuedeCrecer())
                ingresosEsperados += parcela.GetIngresos();
        }

        // Calcular los costos
        ingresosEsperados -= Empleados * Sueldo;

        return ingresosEsperados;
    }

    public float Utilidad ()
        { return Dinero - (Empleados * Sueldo); }
}
