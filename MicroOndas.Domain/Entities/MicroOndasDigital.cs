using MicroOndas.Domain.ValueObjects;
using System;

namespace MicroOndas.Domain.Entities
{
    /// <summary>
    /// Entidade MicroOndas: representa uma operação de aquecimento válida.
    /// </summary>
    public class MicroOndasDigital
    {
        public Tempo Tempo { get; }
        public Potencia Potencia { get; }

        public MicroOndasDigital(Tempo tempo, Potencia potencia)
        {
            Tempo = tempo ?? throw new ArgumentNullException(nameof(tempo));
            Potencia = potencia ?? throw new ArgumentNullException(nameof(potencia));
        }

        /// <summary>
        /// Tempo formatado para exibição (delegado a Tempo.ParaExibir).
        /// </summary>
        public string TempoFormatado => Tempo.ParaExibir();

        /// <summary>
        /// Segundos restantes iniciais (útil para contagem).
        /// </summary>
        public int SegundosIniciais => Tempo.Segundos;
    }
}
