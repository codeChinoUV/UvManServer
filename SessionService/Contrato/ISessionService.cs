using SessionService.Dominio.Enum;
using System.ServiceModel;
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
