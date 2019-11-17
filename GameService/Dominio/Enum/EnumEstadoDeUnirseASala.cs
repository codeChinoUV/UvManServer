
using System.Runtime.Serialization;


namespace GameService.Dominio.Enum
{
    [DataContract]
    public enum EnumEstadoDeUnirseASala
    {
        [EnumMember]
        UnidoCorrectamente = 1,
        [EnumMember]
        SalaInexistente = 0,
        [EnumMember]
        SalaLlena = -1,
        [EnumMember]
        NoSeEncuentraEnSesion = -2
    }
}
