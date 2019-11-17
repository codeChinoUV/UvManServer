
using System.Runtime.Serialization;

namespace GameService.Dominio.Enum
{
    [DataContract]
    public enum EnumEstadoCrearSalaConId
    {
        [EnumMember]
        CreadaCorrectamente = 1,
        [EnumMember]
        IdYaExistente = -1,
        [EnumMember]
        NoSeEncuentraEnSesion = -2
    }
}
