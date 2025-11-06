using MicroOndas.Application.Interfaces;
using MicroOndas.Application.Models;
using MicroOndas.Domain.Entities;
using MicroOndas.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroOndas.Application.Services
{
    public class MicroOndasService : IMicroOndasService
    {
        private readonly List<ProgramaAquecimento> _programas;
        private CancellationTokenSource? _cts;
        private Task? _runningTask;

        private int _tempoRestante;
        private int _potenciaAtual;
        private string _caractereAquecimento = ".";
        private readonly StringBuilder _processBuilder = new();

        public bool EmExecucao { get; private set; }
        public bool Pausado { get; private set; }

        public event Action? OnTick;
        public event Action? OnFinished;

        public MicroOndasService()
        {
            _programas = InicializarProgramas();
        }

        public IReadOnlyList<ProgramaAquecimento> ObterProgramasPreDefinidos() => _programas;

        public void IniciarAquecimento(int tempoSegundos, int potencia)
        {
            // Se já em execução e não pausado -> acrescenta 30s (requisito 5).
            if (EmExecucao && !Pausado)
            {
                // manual: limite de 120s para entrada manual (não deixar ultrapassar)
                _tempoRestante = Math.Min(_tempoRestante + 30, 120);
                return;
            }

            // validações do requisito Nível 1
            if (tempoSegundos < 1 || tempoSegundos > 120)
                throw new ArgumentException("Tempo deve estar entre 1 e 120 segundos para entrada manual.", nameof(tempoSegundos));

            if (potencia < 1 || potencia > 10)
                throw new ArgumentException("Potência deve estar entre 1 e 10.", nameof(potencia));

            _tempoRestante = tempoSegundos;
            _potenciaAtual = potencia;
            _caractereAquecimento = "."; // no manual usamos '.' por requisito
            _processBuilder.Clear();

            StartLoop();
        }

        public void IniciarAquecimento(ProgramaAquecimento programa)
        {
            if (EmExecucao) return;

            // Programas pré-definidos NÃO obedecem limite de 120s.
            _tempoRestante = programa.TempoSegundos;
            _potenciaAtual = programa.Potencia;
            _caractereAquecimento = string.IsNullOrEmpty(programa.CaractereAquecimento) ? "#" : programa.CaractereAquecimento;
            _processBuilder.Clear();

            StartLoop();
        }

        public void Pausar()
        {
            if (!EmExecucao || Pausado) return;
            Pausado = true;
        }

        public void Cancelar()
        {
            _cts?.Cancel();
            ResetState();
        }

        public string ObterDisplay()
        {
            if (!EmExecucao)
                return "Micro-ondas pronto.";

            int mm = _tempoRestante / 60;
            int ss = _tempoRestante % 60;
            string tempo = $"{mm:D2}:{ss:D2}";

            // Mostra também uma pequena representação de potência
            string barras = new string(_caractereAquecimento[0], Math.Max(1, _potenciaAtual));
            return $"{tempo} {barras}";
        }

        public string ObterProcessoVisual() => _processBuilder.ToString();

        // ----------------------------
        // Loop assíncrono
        // ----------------------------
        private void StartLoop()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            EmExecucao = true;
            Pausado = false;

            _runningTask = Task.Run(async () =>
            {
                try
                {
                    while (_tempoRestante > 0 && !token.IsCancellationRequested)
                    {
                        if (!Pausado)
                        {
                            // A cada segundo adicionamos a sequência definida:
                            // se caractereAquecimento for ".", adiciona '.' repetido conforme potência
                            // se for outro caractere (programa), usa esse char.
                            string bloco = new string(_caractereAquecimento[0], Math.Max(1, _potenciaAtual));
                            _processBuilder.Append(bloco);
                            _processBuilder.Append(' ');

                            _tempoRestante--;
                            OnTick?.Invoke();
                        }

                        try
                        {
                            await Task.Delay(1000, token);
                        }
                        catch (TaskCanceledException) { break; }
                    }

                    if (!token.IsCancellationRequested && _tempoRestante <= 0)
                    {
                        // finaliza normalmente
                        EmExecucao = false;
                        Pausado = false;
                        OnFinished?.Invoke();
                        return;
                    }
                }
                catch (OperationCanceledException) { }
                finally
                {
                    // se cancelado, limpamos o cancel token mas não disparamos OnFinished
                    if (token.IsCancellationRequested)
                    {
                        ResetState();
                    }
                }
            }, token);
        }

        private void ResetState()
        {
            EmExecucao = false;
            Pausado = false;
            _tempoRestante = 0;
            _potenciaAtual = 0;
            _caractereAquecimento = ".";
            _processBuilder.Clear();
        }

        private static List<ProgramaAquecimento> InicializarProgramas() => new()
        {
            new ProgramaAquecimento("Pipoca", "Pipoca (de micro-ondas)", 3 * 60, 7, "*",
                "Observar o barulho de estouros. Caso haja intervalo de 10s entre estouros interrompa."),
            new ProgramaAquecimento("Leite", "Leite", 5 * 60, 5, "L",
                "Cuidado com aquecimento de líquidos: choque térmico pode causar fervura imediata."),
            new ProgramaAquecimento("Carnes de boi", "Carne em pedaço ou fatias", 14 * 60, 4, "C",
                "Interrompa na metade e vire o conteúdo para aquecimento uniforme."),
            new ProgramaAquecimento("Frango", "Frango (qualquer corte)", 8 * 60, 7, "F",
                "Interrompa na metade e vire o conteúdo para aquecimento uniforme."),
            new ProgramaAquecimento("Feijão", "Feijão congelado", 8 * 60, 9, "#",
                "Deixe o recipiente destampado; em plástico, cuidado ao retirar.")
        };
    }
}
