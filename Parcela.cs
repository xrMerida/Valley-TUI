namespace Granja;

public class Parcela
{
    private Semilla? Semilla;
    private int MesesSimulados;

    // Parcela vacia
    public Parcela ()
    {
        Semilla = null;
        MesesSimulados = 0;
    }

    public bool EstaLibre ()
    {
        return Semilla == null;
    }
    public Semilla? GetSemilla ()
        { return Semilla; }

    public void SetSemilla (Semilla? semilla)
        {
            MesesSimulados = 0;
            Semilla = semilla;
        }

    public int GetMesesSimulados ()
        { return MesesSimulados; }

    public bool EsCosechable ()
    {
        if (Semilla == null)
            return false;

        // Se le resta uno para tomar en cuenta el mes en el que estara lista
        return MesesSimulados >= Semilla.GetMeses() - 1;
    }
    public decimal GetIngresos ()
    {
        if (Semilla == null)
            return 0M;
        else
            return Semilla.GetIngresos();
    }
    public decimal CosecharYCrecer ()
    {
        if (Semilla == null)
            { return 0M; }

        // Se le resta uno para tomar en cuenta el mes en el que estara lista
        else if (MesesSimulados < Semilla.GetMeses() - 1)
        {
            MesesSimulados++;
            return 0M;
        }

        else
        {
            decimal ingresos = Semilla.GetIngresos();
            SetSemilla(null);
            return ingresos;
        }
    }
}
