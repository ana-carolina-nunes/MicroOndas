using System;

namespace MicroOndas.Domain.ValueObjects
{
    /// <summary>
    /// Value object que representa um tempo seguro (em segundos) para o micro-ondas.
    /// Limites: 1..120 segundos.
    /// </summary>
    public class Tempo
    {
        public int Segundos { get; }

        public Tempo(int segundos)
        {
            if (segundos < 1 || segundos > 120)
                throw new ArgumentOutOfRangeException(nameof(segundos), "O tempo deve estar entre 1 e 120 segundos.");

            Segundos = segundos;
        }

        /// <summary>
        /// Formato para exibição: "s" se <60 ou "m:ss" se >=60.
        /// Ex: 90 -> "1:30", 45 -> "45s"
        /// </summary>
        public string ParaExibir()
        {
            if (Segundos < 60)
                return $"{Segundos}s";

            int minutos = Segundos / 60;
            int segundosRestantes = Segundos % 60;
            return $"{minutos}:{segundosRestantes:D2}";
        }
    }
}
