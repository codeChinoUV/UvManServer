using System.Runtime.Serialization;

namespace MessageService.Dominio.Enum
{
    [DataContract]
    public enum EnumEstadoVerificarCuenta
    {
        [EnumMember]
        VerificadaCorrectamente = 1,
        [EnumMember]
        NoCoincideElCodigo = 0,
        [EnumMember]
        ErrorEnBaseDatos = -1
    }
}
