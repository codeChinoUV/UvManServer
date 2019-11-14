using GameChatService.Contrato;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using GameChatService.Dominio;
using LogicaDelNegocio.Modelo;

namespace GameChatService
{

    [ServiceContract(CallbackContract = typeof(IChatServiceCallback), SessionMode = SessionMode.Required)]
    public interface IChatService
    {
        [OperationContract(IsInitiating = true)]
        bool Conectar(CuentaModel cuenta);

        [OperationContract]
        List<CuentaModel> ObtenerCuentasConectadas();

        [OperationContract(IsOneWay = true)]
        void EnviarMensaje(Message mensaje);

        [OperationContract(IsOneWay = true)]
        void EstaEscribiendo(String cuenta);

        [OperationContract(IsOneWay = true, IsTerminating = true)]
        void Desconectar(CuentaModel cuenta);
    }

    
}
