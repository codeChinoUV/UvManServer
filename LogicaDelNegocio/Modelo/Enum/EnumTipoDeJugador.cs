using System.Runtime.Serialization;

namespace LogicaDelNegocio.Modelo.Enum
{
    [DataContract]
    public enum EnumTipoDeJugador
    {
        [EnumMember]
        Corredor = 0,
        [EnumMember]
        Perseguidor1 = 1,
        [EnumMember]
        Perseguidor2 = 2,
        [EnumMember]
        Perseguidor3 = 3,
        [EnumMember]
        Perseguidor4 = 4
    }
}