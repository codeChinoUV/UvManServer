using System;
using System.ServiceModel;

namespace SessionService.Contrato
{
    [ServiceContract]
    public interface ISessionServiceCallback
    {
        [OperationContract(IsOneWay = false)]
        Boolean EstaVivo();
    }
}
