using System;

namespace Granja;

/// <summary>
/// Simula una sección de la cuadrícula de la granja.
/// Cada parcela puede contener una siembra y lleva el control
/// de sus meses de crecimiento.
/// </summary>
public class Parcela
{
    /// <summary>
    /// Semilla actualmente plantada en la parcela. <c>null</c> si está libre.
    /// </summary>
    public Semilla? Semilla { get; private set; }

    /// <summary>
    /// Meses transcurridos desde que se sembró la parcela.
    /// </summary>
    public int MesesSimulados { get; private set; }

    /// <summary>
    /// Ingresos esperados al cosechar la parcela.
    /// Retorna 0 si la parcela está libre.
    /// </summary>
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

    /// <summary>
    /// Inicializa una parcela vacía con 0 meses simulados.
    /// </summary>
    public Parcela ()
    {
        Semilla = null;
        MesesSimulados = 0;
    }

    /// <summary>
    /// Siembra una semilla en la parcela y reinicia el contador de meses.
    /// </summary>
    /// <param name="semilla">Semilla a plantar. Use <c>null</c> para vaciar la parcela.</param>
    public void Sembrar (Semilla? semilla)
    {
        MesesSimulados = 0;
        Semilla = semilla;
    }

    /// <summary>
    /// Determina si la siembra actual está lista para cosechar.
    /// </summary>
    /// <returns>
    /// <c>true</c> si la parcela tiene semilla y los meses simulados
    /// alcanzaron el tiempo de crecimiento; <c>false</c> en caso contrario.
    /// </returns>
    public bool EsCosechable ()
    {
        if (Semilla == null)
            return false;

        // Se le resta uno para tomar en cuenta el mes en el que estara lista
        return MesesSimulados >= Semilla.Meses - 1;
    }

    /// <summary>
    /// Incrementa en uno los meses simulados de la parcela.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Si la parcela está libre o ya está lista para cosechar.
    /// </exception>
    public void Crecer ()
    {
        if (Semilla == null)
            throw new InvalidOperationException("No se puede crecer una plantacion inexistente");

        if (EsCosechable())
            throw new InvalidOperationException("No se puede crecer una plantacion lista para cosechar");

        // else
        MesesSimulados++;
    }

    /// <summary>
    /// Cosecha la parcela: retorna los ingresos y la deja vacía.
    /// </summary>
    /// <returns>Monto de ingresos generado por la cosecha.</returns>
    /// <exception cref="InvalidOperationException">
    /// Si la parcela está libre o la siembra no está lista para cosechar.
    /// </exception>
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
