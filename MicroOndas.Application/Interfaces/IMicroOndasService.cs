using MicroOndas.Application.Models;
using System;
using System.Collections.Generic;

namespace MicroOndas.Application.Interfaces
{
    public interface IMicroOndasService
    {
        bool EmExecucao { get; }
        bool Pausado { get; }

        /// <summary>
        /// Evento disparado a cada segundo (UI assina e chama StateHasChanged()).
        /// </summary>
        event Action? OnTick;

        /// <summary>
        /// Evento disparado quando o aquecimento termina normalmente.
        /// </summary>
        event Action? OnFinished;

        /// <summary>
        /// Retorna os programas pré-definidos (somente leitura).
        /// </summary>
        IReadOnlyList<ProgramaAquecimento> ObterProgramasPreDefinidos();

        /// <summary>
        /// Inicia aquecimento no modo manual (valida tempo <= 120).
        /// </summary>
        void IniciarAquecimento(int tempoSegundos, int potencia);

        /// <summary>
        /// Inicia aquecimento com um programa pré-definido (sem limite de tempo).
        /// </summary>
        void IniciarAquecimento(ProgramaAquecimento programa);

        /// <summary>
        /// Pausa o aquecimento.
        /// </summary>
        void Pausar();

        /// <summary>
        /// Cancela o aquecimento e limpa estado.
        /// </summary>
        void Cancelar();

        /// <summary>
        /// Retorna o texto do display (ex: "00:30 ***").
        /// </summary>
        string ObterDisplay();

        /// <summary>
        /// Retorna a string visual (linhas de caracteres) do processo montado até agora.
        /// </summary>
        string ObterProcessoVisual();
    }
}
