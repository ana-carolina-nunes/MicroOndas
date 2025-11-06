using MicroOndas.Application.Models;
using System;
using System.Collections.Generic;

namespace MicroOndas.Application.Interfaces
{
    public interface IMicroOndasService
    {
        /// <summary>
        /// Indica se o micro-ondas está em execução (contagem de tempo ativa).
        /// </summary>
        bool EmExecucao { get; }

        /// <summary>
        /// Indica se o micro-ondas está pausado.
        /// </summary>
        bool Pausado { get; }

        /// <summary>
        /// Evento acionado a cada segundo de aquecimento.
        /// A UI assina este evento e chama StateHasChanged().
        /// </summary>
        event Action? OnTick;

        /// <summary>
        /// Evento acionado quando o tempo chega a zero sem cancelamento.
        /// A UI usa para exibir "Aquecimento concluído".
        /// </summary>
        event Action? OnFinished;

        /// <summary>
        /// Retorna a lista dos programas pré-definidos, que não podem ser alterados.
        /// </summary>
        IReadOnlyList<ProgramaAquecimento> ObterProgramasPreDefinidos();

        /// <summary>
        /// Inicia aquecimento no modo manual (tempo e potência digitados).
        /// Limite de 120 segundos é validado aqui.
        /// </summary>
        void IniciarAquecimento(int tempoSegundos, int potencia);

        /// <summary>
        /// Inicia aquecimento com um dos programas pré-definidos.
        /// Sem limite de tempo.
        /// </summary>
        void IniciarAquecimento(ProgramaAquecimento programa);

        /// <summary>
        /// Pausa o aquecimento, podendo ser retomado.
        /// </summary>
        void Pausar();

        /// <summary>
        /// Cancela o aquecimento e limpa estado.
        /// </summary>
        void Cancelar();

        /// <summary>
        /// Retorna o texto que deve aparecer no display na interface (ex: "00:20 .....").
        /// </summary>
        string ObterDisplay();
    }
}
