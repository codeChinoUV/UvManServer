using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using LogicaDelNegocio.Modelo;
using MessageService.Dominio.Enum;

namespace CuentaService.Contrato
{
    [ServiceContract]
    public interface ICuentaService
    {
        [OperationContract]
        EnumEstadoRegistro CheckIn(CuentaModel newUser);

        [OperationContract]
        EnumEstadoVerificarCuenta VerifyAccount(String code, CuentaModel cuenta);
    }
}
