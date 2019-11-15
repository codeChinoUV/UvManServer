using SessionService.Dominio;
using SessionService.Dominio.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using LogicaDelNegocio.Modelo;

namespace SessionService.Contrato
{

    [ServiceContract(CallbackContract = typeof(ISessionServiceCallback), SessionMode = SessionMode.Required)]
    public interface ISessionService
    {
        [OperationContract(IsInitiating = true)]
        EnumEstadoInicioSesion IniciarSesion(CuentaModel Cuenta);

        [OperationContract(IsTerminating = true)]
        void CerrarSesion(CuentaModel Cuenta);
    }
}
