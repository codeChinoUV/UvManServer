using LogicaDelNegocio.Modelo;

namespace GameService.Contrato
{
    public interface IGameServiceCallback
    {
        void TerminarPartida();

        void SalaLlena();

        void NuevoCuentaEnLaSala(CuentaModel cuenta);
    }
}
