using GameService.Dominio.Enum;
using   LogicaDelNegocio.Modelo;
using System;
using System.Collections.Generic;
using System.ServiceModel;


namespace GameService.Contrato
{
    [ServiceContract(CallbackContract = typeof(IGameServiceCallback))]
    public interface IGameService
    {
        [OperationContract]
        EnumEstadoDeUnirseASala UnirseASalaPrivada(String Id, CuentaModel Cuenta);
        [OperationContract]
        Boolean UnirseASala(CuentaModel Cuenta);
        [OperationContract]
        EnumEstadoCrearSalaConId CrearSala(String Id, Boolean EsSalaPublica, CuentaModel Cuenta);
        [OperationContract]
        Boolean VerificarSiEstoyEnSala(CuentaModel Cuenta);
        [OperationContract]
        List<CuentaModel> ObtenerCuentasEnMiSala(CuentaModel Cuenta);
        [OperationContract]
        String RecuperarIdDeMiSala(CuentaModel Cuenta);
        [OperationContract]
        bool MiSalaEsPublica(CuentaModel Cuenta);
        [OperationContract]
        void TerminarPartida(CuentaModel CuentaDeCorredor);
        [OperationContract]
        void NotificarIniciarNivel(CuentaModel CuentaDelCorredor);
        [OperationContract]
        List<CuentaModel> RecuperarMejoresPuntuaciones();
        [OperationContract]
        void EstaLaSalaLlena(CuentaModel CuentaEnSala);
    }
}
