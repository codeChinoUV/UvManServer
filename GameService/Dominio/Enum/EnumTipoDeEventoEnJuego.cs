using System;
using System.Runtime.Serialization;

namespace GameService.Dominio.Enum
{
    [Serializable]
    public enum EnumTipoDeEventoEnJuego
    {
        IniciarCuentaRegresivaInicioJuego = 0,
        IniciarPartida = 1,
        MovimientoJugador = 2,
        MuerteJugador = 3,
        IniciarCuentaRegresivaInicioNivel = 4,
        IniciarNivel = 5,
        IniciaTiempoMatar = 6
    }
}