using GameChatService.Dominio;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using LogicaDelNegocio.Modelo;

namespace GameChatService.Contrato
{
    [ServiceContract]
    public interface IChatServiceCallback
    {

        [OperationContract(IsOneWay = true)]
        void RecibirMensaje(Message Mensaje);

        [OperationContract(IsOneWay = true)]
        void Unirse(CuentaModel Cuenta);

        [OperationContract(IsOneWay = true)]
        void Abandonar(CuentaModel Cuenta);
    }
}
