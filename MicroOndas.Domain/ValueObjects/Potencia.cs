using System;
namespace MicroOndas.Domain.ValueObjects
{
    /// <summary>
    /// Value object para potência (1..10). Se null -> assume 10.
    /// </summary>
    public class Potencia
    {
        public int Valor { get; }

        public Potencia(int? valor)
        {
            Valor = valor ?? 10;

            if (Valor < 1 || Valor > 10)
                throw new ArgumentOutOfRangeException(nameof(valor), "A potência deve estar entre 1 e 10.");
        }

        public override string ToString() => Valor.ToString();
    }
}
