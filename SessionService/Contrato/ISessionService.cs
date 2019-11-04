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
    
    [ServiceContract]
    public interface ISessionService
    {
        [OperationContract]
        EnumEstadoInicioSesion IniciarSesion(CuentaModel cuenta);

        [OperationContract]
        void CerrarSesion(CuentaModel cuenta);
        
    }
}
