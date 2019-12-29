using System;
using System.ServiceModel;
using LogicaDelNegocio.Modelo;
using MessageService.Dominio.Enum;

namespace CuentaService.Contrato
{
    [ServiceContract]
    public interface ICuentaService
    {
        [OperationContract]
        EnumEstadoRegistro Registrarse(CuentaModel CuentaNueva);
        [OperationContract]
        EnumEstadoVerificarCuenta VerificarCuenta(String CodigoDeVerificacion, CuentaModel CuentaAVerificar);
        [OperationContract]
        void ReEnviarCorreoVerificacion(CuentaModel Cuenta);
    }
}
