using System;

namespace Granja;

public class Parcela
{
    public Semilla? Semilla { get; private set; }
    public int MesesSimulados { get; private set; }
    public decimal Ingresos
    {
        get
        {
            if (Semilla == null)
                return 0M;

            // else
            return Semilla.Ingresos;
        }
    }

    public Parcela ()
    {
        Semilla = null;
        MesesSimulados = 0;
    }

    public bool EstaLibre ()
        { return Semilla == null; }

    public void Sembrar (Semilla? semilla)
    {
        MesesSimulados = 0;
        Semilla = semilla;
    }

    public bool EsCosechable ()
    {
        if (Semilla == null)
            return false;

        // Se le resta uno para tomar en cuenta el mes en el que estara lista
        return MesesSimulados >= Semilla.Meses - 1;
    }
    public void Crecer ()
    {
        if (Semilla == null)
            throw new InvalidOperationException("No se puede crecer una plantacion inexistente");

        if (EsCosechable())
            throw new InvalidOperationException("No se puede crecer una plantacion lista para cosechar");

        // else
        MesesSimulados++;
    }

    public decimal Cosechar ()
    {
        if (Semilla == null)
            throw new InvalidOperationException("No se puede cosechar una plantacion inexistente");

        if (!EsCosechable())
            throw new InvalidOperationException("No se puede cosechar una plantacion que no esta lista");

        // else
        decimal ingresos = Semilla.Ingresos;
        // Se elimina la semilla (parcela vacia)
        Sembrar(null);
        return ingresos;
    }
}
