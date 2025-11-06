using MicroOndas.Application.Interfaces;
using MicroOndas.Application.Models;
using MicroOndas.Domain.Entities;
using MicroOndas.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MicroOndas.Application.Services
{
    public class MicroOndasService : IMicroOndasService
    {
        private readonly List<ProgramaAquecimento> _programasPreDefinidos;
        private readonly List<ProgramaAquecimento> _customPrograms = new();
        private readonly string _storageFilePath;

        // execução
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
            _programasPreDefinidos = InicializarProgramas();

      
            // Caminho atual (da pasta onde o projeto está rodando)
            string currentPath = Directory.GetCurrentDirectory();

            // Sobe 1 nível 
            string solutionPath = Directory.GetParent(currentPath).FullName;

            //definir local de persistência simples
            if (!Directory.Exists(solutionPath)) Directory.CreateDirectory(solutionPath);
            _storageFilePath = Path.Combine(solutionPath, "programas_custom.json");

            LoadCustomPrograms();
        }

        public IReadOnlyList<ProgramaAquecimento> ObterProgramasPreDefinidos() => _programasPreDefinidos;

        public IReadOnlyList<ProgramaAquecimento> ObterTodosProgramas()
        {
            // pré-definidos primeiro, depois os custom (para exibição)
            return _programasPreDefinidos.Concat(_customPrograms).ToList();
        }

        public void AdicionarProgramaCustomizado(ProgramaAquecimento programa)
        {
            // validações obrigatórias
            if (programa is null) throw new ArgumentNullException(nameof(programa));
            if (string.IsNullOrWhiteSpace(programa.Nome)) throw new ArgumentException("Nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(programa.Alimento)) throw new ArgumentException("Alimento é obrigatório.");
            if (programa.TempoSegundos <= 0) throw new ArgumentException("Tempo deve ser maior que 0.");
            if (programa.Potencia < 1 || programa.Potencia > 10) throw new ArgumentException("Potência deve estar entre 1 e 10.");
            if (string.IsNullOrWhiteSpace(programa.CaractereAquecimento)) throw new ArgumentException("Caractere de aquecimento é obrigatório.");
            if (programa.CaractereAquecimento == ".") throw new ArgumentException("Caractere '.' é reservado e não pode ser usado.");
            if (programa.CaractereAquecimento.Length != 1) throw new ArgumentException("Caractere de aquecimento deve ser exatamente 1 caractere.");

            // unicidade do caractere entre TODOS os programas
            var todos = ObterTodosProgramas();
            if (todos.Any(p => p.CaractereAquecimento == programa.CaractereAquecimento))
                throw new ArgumentException($"Caractere '{programa.CaractereAquecimento}' já está em uso por outro programa.");

            // nome também pode ser verificado (opcional)
            if (todos.Any(p => string.Equals(p.Nome, programa.Nome, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Já existe um programa com o nome '{programa.Nome}'.");

            // cria cópia com IsCustom = true
            var custom = new ProgramaAquecimento(programa.Nome, programa.Alimento, programa.TempoSegundos, programa.Potencia, programa.CaractereAquecimento, programa.Instrucoes, true);
            _customPrograms.Add(custom);
            SaveCustomPrograms();
        }

        private void SaveCustomPrograms()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var dto = _customPrograms.Select(p => new
                {
                    p.Nome,
                    p.Alimento,
                    p.TempoSegundos,
                    p.Potencia,
                    p.CaractereAquecimento,
                    p.Instrucoes
                }).ToList();

                var json = JsonSerializer.Serialize(dto, options);
                File.WriteAllText(_storageFilePath, json, Encoding.UTF8);
            }
            catch
            {
                // não lançar exceção para não quebrar a UI; poderia logar em produção
            }
        }

        private void LoadCustomPrograms()
        {
            try
            {
                if (!File.Exists(_storageFilePath)) return;

                var json = File.ReadAllText(_storageFilePath, Encoding.UTF8);
                var doc = JsonSerializer.Deserialize<List<CustomDto>>(json);
                if (doc is null) return;

                _customPrograms.Clear();
                foreach (var d in doc)
                {
                    // validar caractere também ao carregar (se inválido, pular)
                    if (string.IsNullOrWhiteSpace(d.CaractereAquecimento) || d.CaractereAquecimento.Length != 1) continue;
                    // não permitir '.' ao carregar
                    if (d.CaractereAquecimento == ".") continue;

                    // garantir unicidade em relação a pré-definidos e já carregados
                    var todos = _programasPreDefinidos.Concat(_customPrograms);
                    if (todos.Any(p => p.CaractereAquecimento == d.CaractereAquecimento)) continue;

                    var p = new ProgramaAquecimento(d.Nome, d.Alimento, d.TempoSegundos, d.Potencia, d.CaractereAquecimento, d.Instrucoes, true);
                    _customPrograms.Add(p);
                }
            }
            catch
            {
                // ignorar problemas de leitura; em produção logar
            }
        }

        private class CustomDto
        {
            public string Nome { get; set; } = "";
            public string Alimento { get; set; } = "";
            public int TempoSegundos { get; set; }
            public int Potencia { get; set; }
            public string CaractereAquecimento { get; set; } = "";
            public string Instrucoes { get; set; } = "";
        }

        public void IniciarAquecimento(int tempoSegundos, int potencia)
        {
            // Se já estava rodando e apenas pausado → retomar
            if (EmExecucao && Pausado)
            {
                Pausado = false;
                //StartLoop(); // retoma com _tempoRestante intacto
                return;
            }

            // Se já está rodando e NÃO está pausado → só adiciona +30s (regra do projeto)
            if (EmExecucao && !Pausado)
            {
                _tempoRestante = Math.Min(_tempoRestante + 30, 120);
                return;
            }

            // Novo aquecimento > aplica lógica padrão de inicialização
            potencia = potencia == 0 ? 10 : potencia;
            tempoSegundos = tempoSegundos == 0 ? 30 : tempoSegundos;

            if (tempoSegundos < 1 || tempoSegundos > 120)
                throw new ArgumentException("Tempo deve estar entre 1 e 120 segundos para entrada manual.");
            if (potencia < 1 || potencia > 10)
                throw new ArgumentException("Potência deve estar entre 1 e 10.");

            _tempoRestante = tempoSegundos;
            _potenciaAtual = potencia;
            _caractereAquecimento = ".";
            _processBuilder.Clear();
            Pausado = false;

            StartLoop();
        }


        public void IniciarAquecimento(ProgramaAquecimento programa)
        {
            // Se já estava rodando e pausado > retomar sem resetar nada
            if (EmExecucao && Pausado)
            {
                Pausado = false;
                //StartLoop();
                return;
            }

            // Se já está executando e NÃO está pausado → ignorar (não substitui programa)
            if (EmExecucao)
                return;

            // Novo programa → carrega normalmente
            _tempoRestante = programa.TempoSegundos;
            _potenciaAtual = programa.Potencia;
            _caractereAquecimento = programa.CaractereAquecimento;
            _processBuilder.Clear();
            Pausado = false;

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
            if (!EmExecucao) return "Micro-ondas disponível.";
            int mm = _tempoRestante / 60;
            int ss = _tempoRestante % 60;
            string tempo = $"{mm:D2}:{ss:D2}";
            string barras = new string(_caractereAquecimento[0], Math.Max(1, _potenciaAtual));
            return $"{tempo} {barras}";
        }

        public string ObterProcessoVisual() => _processBuilder.ToString();

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
                        EmExecucao = false;
                        Pausado = false;
                        OnFinished?.Invoke();
                        return;
                    }
                }
                catch (OperationCanceledException) { }
                finally
                {
                    if (token.IsCancellationRequested) ResetState();
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

        private string FormatTime(int totalSeconds)
        {
            if (totalSeconds < 60)
                return totalSeconds.ToString(); 

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes}:{seconds:D2}"; 
        }


        private static List<ProgramaAquecimento> InicializarProgramas() => new()
        {
            new ProgramaAquecimento("Pipoca", "Pipoca (de micro-ondas)", 3 * 60, 7, "*",
                "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento."),
            new ProgramaAquecimento("Leite", "Leite", 5 * 60, 5, "L",
                "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras."),
            new ProgramaAquecimento("Carnes de boi", "Carne em pedaço ou fatias", 14 * 60, 4, "C",
                "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme."),
            new ProgramaAquecimento("Frango", "Frango (qualquer corte)", 8 * 60, 7, "F",
                "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme."),
            new ProgramaAquecimento("Feijão", "Feijão congelado", 8 * 60, 9, "#",
                "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.")
        };
    }
}
