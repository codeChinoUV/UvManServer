using GameChatService.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameChatService.Contrato
{
    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void RefrescarCuentasConectadas(List<Cuenta> cuentasConectadas);

        [OperationContract(IsOneWay = true)]
        void RecibirMensaje(Message mensaje);

        [OperationContract(IsOneWay = true)]
        void EstaEscribiendoCallback(Cuenta cuenta);

        [OperationContract(IsOneWay = true)]
        void Unirse(Cuenta cuenta);

        [OperationContract(IsOneWay = true)]
        void Abandonar(Cuenta cuenta);
    }
}
