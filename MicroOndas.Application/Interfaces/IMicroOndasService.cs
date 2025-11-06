using MicroOndas.Domain.Entities;

namespace MicroOndas.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação que valida e cria uma operação de aquecimento.
    /// </summary>
    public interface IMicroOndasService
    {
        /// <summary>
        /// Valida entradas e cria a entidade MicroOndas.
        /// </summary>
        MicroOndasDigital Iniciar(int tempoSegundos, int? potencia);
    }
}
