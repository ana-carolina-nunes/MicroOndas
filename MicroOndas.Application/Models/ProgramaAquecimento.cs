namespace MicroOndas.Application.Models
{
    public sealed class ProgramaAquecimento
    {
        public string Nome { get; }
        public string Alimento { get; }
        public int TempoSegundos { get; } 
        public int Potencia { get; } 
        public string CaractereAquecimento { get; } 
        public string Instrucoes { get; }
        public bool IsCustom { get; }

        /// <summary>
        /// Prgrama Aquecimento
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="alimento"></param>
        /// <param name="tempoSegundos">pode exceder 120 para programas</param>
        /// <param name="potencia"> 1..10</param>
        /// <param name="caractere"></param>
        /// <param name="instrucoes"></param>
        /// <param name="isCustom"></param>
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
