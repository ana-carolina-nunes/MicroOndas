namespace MicroOndas.Application.Models
{
    public sealed class ProgramaAquecimento
    {
        public string Nome { get; }
        public string Alimento { get; }
        public int TempoSegundos { get; } // pode exceder 120 para programas
        public int Potencia { get; } // 1..10
        public string CaractereAquecimento { get; } // não pode ser "."
        public string Instrucoes { get; }
        public bool IsCustom { get; } // true se criado pelo usuário

        public ProgramaAquecimento(string nome, string alimento, int tempoSegundos, int potencia, string caractere, string instrucoes, bool isCustom = false)
        {
            Nome = nome;
            Alimento = alimento;
            TempoSegundos = tempoSegundos;
            Potencia = potencia;
            CaractereAquecimento = caractere;
            Instrucoes = instrucoes;
            IsCustom = isCustom;
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
