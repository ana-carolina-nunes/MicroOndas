using MicroOndas.Application.Interfaces;
using MicroOndas.Application.Models;
using MicroOndas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MicroOndas.Application.Services
{
    public class MicroOndasService : IMicroOndasService
    {
        private readonly List<ProgramaAquecimento> _programas;
        private CancellationTokenSource? _cts;
        private Task? _loopTask;

        private int _tempoRestante;
        private int _potenciaAtual;
        private string _caractereAquecimento = ".";

        public bool EmExecucao { get; private set; }
        public bool Pausado { get; private set; }

        // 🔔 Evento disparado a cada segundo para atualizar UI
        public event Action? OnTick;

        // 🔔 Evento disparado ao final do aquecimento
        public event Action? OnFinished;

        public MicroOndasService()
        {
            _programas = InicializarProgramas();
        }

        public IReadOnlyList<ProgramaAquecimento> ObterProgramasPreDefinidos() => _programas;

        // ============================
        // 📌 INÍCIO MANUAL / RÁPIDO
        // ============================
        public void IniciarAquecimento(int tempoSegundos, int potencia)
        {
            if (EmExecucao && !Pausado)
            {
                // ✅ Regra: acrescentar +30 segundos se já está aquecendo
                _tempoRestante = Math.Min(_tempoRestante + 30, 120);
                return;
            }

            if (tempoSegundos <= 0)
                throw new InvalidOperationException("Tempo deve ser maior que zero.");

            if (tempoSegundos > 120)
                throw new InvalidOperationException("Tempo máximo permitido no modo manual é 120 segundos.");

            if (potencia < 1 || potencia > 10)
                throw new InvalidOperationException("A potência deve estar entre 1 e 10.");

            _tempoRestante = tempoSegundos;
            _potenciaAtual = potencia;
            _caractereAquecimento = new string('.', potencia); // padrão

            IniciarLoop();
        }

        // ============================
        // 📌 INÍCIO PROGRAMA PRE-DEFINIDO
        // ============================
        public void IniciarAquecimento(ProgramaAquecimento programa)
        {
            if (EmExecucao) return;

            _tempoRestante = programa.TempoSegundos;
            _potenciaAtual = programa.Potencia;
            _caractereAquecimento = programa.CaractereAquecimento;

            IniciarLoop();
        }

        // ============================
        // 📌 CONTROLE
        // ============================
        public void Pausar()
        {
            if (!EmExecucao || Pausado) return;
            Pausado = true;
        }

        public void Cancelar()
        {
            _cts?.Cancel();
            Reset();
        }

        private void Reset()
        {
            EmExecucao = false;
            Pausado = false;
            _tempoRestante = 0;
        }

        // ============================
        // 🔁 LOOP ASSÍNCRONO
        // ============================
        private void IniciarLoop()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            EmExecucao = true;
            Pausado = false;

            _loopTask = Task.Run(async () =>
            {
                try
                {
                    while (_tempoRestante > 0)
                    {
                        if (token.IsCancellationRequested) break;

                        if (!Pausado)
                        {
                            _tempoRestante--;
                            OnTick?.Invoke(); // 🔔 Notifica UI
                        }

                        await Task.Delay(1000, token);
                    }

                    if (!token.IsCancellationRequested)
                    {
                        Reset();
                        OnFinished?.Invoke(); // 🔔 Final completo
                    }
                }
                catch (TaskCanceledException) { }
            }, token);
        }

        // ============================
        // ⏱ EXIBIR DISPLAY
        // ============================
        public string ObterDisplay()
        {
            if (!EmExecucao)
                return "Micro-ondas pronto.";

            int min = _tempoRestante / 60;
            int sec = _tempoRestante % 60;

            string tempo = $"{min:00}:{sec:00}";
            string barras = new string(_caractereAquecimento[0], _potenciaAtual);
            return $"{tempo} {barras}";
        }

        // ============================
        // 📦 PROGRAMAS FIXOS
        // ============================
        private static List<ProgramaAquecimento> InicializarProgramas() =>
            new()
            {
                new ProgramaAquecimento("Pipoca", "Pipoca (de micro-ondas)", 180, 7, "P",
                    "Observar estouros: se passar 10s sem barulho, interrompa."),
                new ProgramaAquecimento("Leite", "Leite", 300, 5, "L",
                    "Cuidado com líquidos: risco de fervura instantânea."),
                new ProgramaAquecimento("Carnes de boi", "Carne em pedaço/fatias", 840, 4, "C",
                    "Pause na metade e vire para aquecimento uniforme."),
                new ProgramaAquecimento("Frango", "Frango (qualquer corte)", 480, 7, "F",
                    "Pause na metade e vire para aquecimento uniforme."),
                new ProgramaAquecimento("Feijão", "Feijão congelado", 480, 9, "#",
                    "Recipiente destampado. Plástico pode perder resistência.")
            };
    }
}
