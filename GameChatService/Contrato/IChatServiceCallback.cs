using GameChatService.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using LogicaDelNegocio.Modelo;

namespace GameChatService.Contrato
{
    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void RefrescarCuentasConectadas(List<CuentaModel> cuentasConectadas);

        [OperationContract(IsOneWay = true)]
        void RecibirMensaje(Message mensaje);

        [OperationContract(IsOneWay = true)]
        void EstaEscribiendoCallback(String cuenta);

        [OperationContract(IsOneWay = true)]
        void Unirse(CuentaModel cuenta);

        [OperationContract(IsOneWay = true)]
        void Abandonar(CuentaModel cuenta);
    }
}
