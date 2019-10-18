using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using CuentaService.Dominio;

namespace CuentaService.Contrato
{
    [ServiceContract]
    public interface ICuentaService
    {
        [OperationContract]
        int CheckIn(Cuenta newUser);

        [OperationContract]
        int VerifyAccount(String code, Cuenta cuenta);
    }
}
