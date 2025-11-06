using System;

namespace MicroOndas.Domain.ValueObjects
{
    /// <summary>
    /// Value object que representa um tempo (em segundos).
    /// Limites: 1..120 (validação apenas para entrada manual).
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

        public string ParaExibir()
        {
            if (Segundos < 60)
                return $"{Segundos:D2}s";

            int minutos = Segundos / 60;
            int segundosRestantes = Segundos % 60;
            return $"{minutos:D2}:{segundosRestantes:D2}";
        }
    }
}
