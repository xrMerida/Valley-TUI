using System;

namespace Granja;

public class Parcela
{
    private Cultivo? Semilla;
    private int MesesSimulados;

    public Parcela (int fila, int columna, Cultivo semilla)
    {
        if (fila < 0)
            throw new ArgumentException("Fila debe ser un indice de arreglo valido", nameof(fila));

        if (columna < 0)
            throw new ArgumentException("Columna debe ser un indice de arreglo valido", nameof(columna));

        Semilla = semilla;
        MesesSimulados = 0;
    }
    public Cultivo? GetSemilla ()
        { return Semilla; }

    public void SetSemilla (Cultivo semilla)
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

    public float GetIngresos ()
    {
        if (Semilla != null)
            return Semilla.GetIngresos();
        else
            return 0f;
    }
    public float Cosechar ()
    {
        if (Semilla == null)
            return 0f;

        if (MesesSimulados < Semilla.GetMeses())
            return 0f;

        float ingresos = Semilla.GetIngresos();
        Semilla = null;
        return ingresos;
    }
}
