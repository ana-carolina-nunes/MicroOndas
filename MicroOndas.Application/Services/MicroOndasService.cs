using MicroOndas.Application.Interfaces;
using MicroOndas.Domain.Entities;
using MicroOndas.Domain.ValueObjects;

namespace MicroOndas.Application.Services
{
    public class MicroOndasService : IMicroOndasService
    {
        public MicroOndasDigital Iniciar(int tempoSegundos, int? potencia)
        {
            // VO fazem validações internas
            var tempo = new Tempo(tempoSegundos);
            var pot = new Potencia(potencia);

            return new MicroOndasDigital(tempo, pot);
        }
    }
}
