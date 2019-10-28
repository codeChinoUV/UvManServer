using GameChatService.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GameChatService.Dominio;

namespace GameChatService
{

    [ServiceContract(CallbackContract = typeof(IChatServiceCallback), SessionMode = SessionMode.Required)]
    public interface IChatService
    {
        [OperationContract(IsInitiating = true)]
        bool Conectar(Cuenta cuenta);

        [OperationContract]
        List<Cuenta> ObtenerCuentasConectadas();

        [OperationContract(IsOneWay = true)]
        void EnviarMensaje(Message mensaje);

        [OperationContract(IsOneWay = true)]
        void EstaEscribiendo(Cuenta cuenta);

        [OperationContract(IsOneWay = true, IsTerminating = true)]
        void Desconectar(Cuenta cuenta);
    }

    
}
