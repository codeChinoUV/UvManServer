using System.Collections.Generic;
using LogicaDelNegocio.Modelo;
using System.ServiceModel;

namespace GameService.Contrato
{
    [ServiceContract]
    public interface IGameServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void TerminarPartida();
        [OperationContract(IsOneWay = true)]
        void SalaLlena();
        [OperationContract(IsOneWay = true)]
        void NuevoCuentaEnLaSala(CuentaModel cuenta);
        [OperationContract(IsOneWay = true)]
        void CuentaAbandoSala(CuentaModel cuenta);
        [OperationContract(IsOneWay = true)]
        void RefrescarCuentasEnSala(List<CuentaModel> CuentasEnMiSala);
    }
}
