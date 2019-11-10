using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SessionService.Contrato
{
    public interface ISessionServiceCallback
    {
        [OperationContract(IsOneWay = false)]
        Boolean EstaVivo();
    }
}
