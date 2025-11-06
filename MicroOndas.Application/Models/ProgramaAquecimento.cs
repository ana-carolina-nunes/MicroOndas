namespace MicroOndas.Application.Models
{
    /// <summary>
    /// Representa um programa de aquecimento pré-definido.
    /// </summary>
    public class ProgramaAquecimento
    {
        public string Nome { get; }
        public string Alimento { get; }
        public int TempoSegundos { get; }
        public int Potencia { get; }
        public string CaractereAquecimento { get; }
        public string Instrucoes { get; }

        public ProgramaAquecimento(
            string nome,
            string alimento,
            int tempoSegundos,
            int potencia,
            string caractereAquecimento,
            string instrucoes)
        {
            Nome = nome;
            Alimento = alimento;
            TempoSegundos = tempoSegundos;
            Potencia = potencia;
            CaractereAquecimento = caractereAquecimento;
            Instrucoes = instrucoes;
        }

        public override string ToString() =>
            $"{Nome} - {Alimento}";
    }
}
