using System.Runtime.Serialization;

namespace GameService.Dominio.Enum
{
    [DataContract]
    public enum EnumTipoDeEventoEnJuego
    {
        [EnumMember]
        CambiarPantalla = 0,
        [EnumMember]
        IniciarPartida = 1,
        [EnumMember]
        MovimientoJugador = 2,
        [EnumMember]
        MuerteJugador = 3,
    }
}