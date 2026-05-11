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

        return MesesSimulados >= Semilla.GetMeses();
    }
    public bool Crecer ()
    {
        if (!EsCosechable())
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
        MesesSimulados = 0;
        return ingresos;
    }
}
