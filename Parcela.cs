using System;

namespace Granja;

public class Parcela
{
    private Semilla? Semilla;
    private int MesesSimulados;

    public Parcela (int fila, int columna, Semilla semilla)
    {
        if (fila < 0)
            throw new ArgumentException("Fila debe ser un indice de arreglo valido", nameof(fila));

        if (columna < 0)
            throw new ArgumentException("Columna debe ser un indice de arreglo valido", nameof(columna));

        Semilla = semilla;
        MesesSimulados = 0;
    }
    public Semilla? GetSemilla ()
        { return Semilla; }

    public void SetSemilla (Semilla semilla)
        { Semilla = semilla; }

    public int GetMesesSimulados ()
        { return MesesSimulados; }

    public bool PuedeCrecer ()
    {
        if (Semilla == null)
            return false;

        return MesesSimulados >= Semilla.GetMeses();
    }
    public bool Crecer ()
    {
        if (!PuedeCrecer())
            return false;

        // else
        MesesSimulados++;
        return true;
    }

    public decimal GetIngresos ()
    {
        if (Semilla != null)
            return Semilla.GetIngresos();
        else
            return 0M;
    }
    public decimal Cosechar ()
    {
        if (Semilla == null)
            return 0M;

        if (MesesSimulados < Semilla.GetMeses())
            return 0M;

        decimal ingresos = Semilla.GetIngresos();
        Semilla = null;
        return ingresos;
    }
}
