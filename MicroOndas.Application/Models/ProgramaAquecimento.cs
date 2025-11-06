namespace MicroOndas.Application.Models
{
    /// <summary>
    /// Representa um programa pré-definido de aquecimento.
    /// </summary>
    public sealed class ProgramaAquecimento
    {
        public string Nome { get; }
        public string Alimento { get; }
        public int TempoSegundos { get; } // pode exceder 120 para programas
        public int Potencia { get; } // 1..10
        public string CaractereAquecimento { get; } // não pode ser "."
        public string Instrucoes { get; }

        public ProgramaAquecimento(string nome, string alimento, int tempoSegundos, int potencia, string caractere, string instrucoes)
        {
            Nome = nome;
            Alimento = alimento;
            TempoSegundos = tempoSegundos;
            Potencia = potencia;
            CaractereAquecimento = caractere;
            Instrucoes = instrucoes;
        }

        public string TempoFormatado
        {
            get
            {
                int mm = TempoSegundos / 60;
                int ss = TempoSegundos % 60;
                return $"{mm:D2}:{ss:D2}";
            }
        }

        public override string ToString() => $"{Nome} - {Alimento}";
    }
}
