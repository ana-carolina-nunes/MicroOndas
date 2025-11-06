using MicroOndas.Domain.ValueObjects;
using System;

namespace MicroOndas.Domain.Entities
{
    /// <summary>
    /// Entidade que representa uma operação válida de aquecimento.
    /// </summary>
    public sealed class MicroOndasDigital
    {
        public Tempo Tempo { get; }
        public Potencia Potencia { get; }

        public MicroOndasDigital(Tempo tempo, Potencia potencia)
        {
            Tempo = tempo ?? throw new ArgumentNullException(nameof(tempo));
            Potencia = potencia ?? throw new ArgumentNullException(nameof(potencia));
        }

        public string TempoFormatado => Tempo.ParaExibir();
        public int SegundosIniciais => Tempo.Segundos;
    }
}
