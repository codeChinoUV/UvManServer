using System.ServiceModel;
using GameChatService.Dominio;
using LogicaDelNegocio.Modelo;

namespace GameChatService.Contrato
{

    [ServiceContract(CallbackContract = typeof(IChatServiceCallback), SessionMode = SessionMode.Required)]
    public interface IChatService
    {
        [OperationContract(IsInitiating = true)]
        bool Conectar(CuentaModel Cuenta);

        [OperationContract(IsOneWay = true)]
        void EnviarMensaje(Message Mensaje);

        [OperationContract(IsOneWay = true, IsTerminating = true)]
        void Desconectar(CuentaModel Cuenta);
    }

    
}
