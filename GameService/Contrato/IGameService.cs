using GameService.Dominio.Enum;
using LogicaDelNegocio.Modelo;
using System;
using System.ServiceModel;


namespace GameService.Contrato
{
    [ServiceContract(CallbackContract = typeof(IGameServiceCallback))]
    public interface IGameService
    {
        [OperationContract]
        EnumEstadoDeUnirseASala UnirseASala(String Id, CuentaModel Cuenta);
        [OperationContract]
        Boolean UnirseASala(CuentaModel Cuenta);
        [OperationContract]
        EnumEstadoCrearSalaConId CrearSala(String Id, Boolean EsSalaPublica, CuentaModel Cuenta);
        [OperationContract]
        Boolean VerificarSiEstoyEnSala(CuentaModel Cuenta);
    }
}
