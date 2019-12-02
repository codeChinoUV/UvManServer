using System;
using GameService.Dominio.Enum;

namespace GameService.Dominio
{
    [Serializable]
    public class EventoEnJuego
    {
        
        public EnumTipoDeEventoEnJuego TipoDeEvento { get; set; }
       
        public MovimientoJugador DatosDelMovimiento { get; set; }
        
        public MuerteJugador DatosMuerteDeUnJugador { get; set; }
        
        public InicioPartida DatosInicioDePartida { set; get; }
        
        public int Ping { get; set; }

        public void EventoEnJuegoMuerteJugador(String usuario, int cantidadDeVidas, int ping)
        {
            TipoDeEvento = EnumTipoDeEventoEnJuego.MuerteJugador;
            DatosMuerteDeUnJugador = new MuerteJugador(usuario, cantidadDeVidas);
            Ping = ping;
        }

        public void EventoEnJuegoMovimientoJugador(String usuario, float posicionX, float posicionY, String movimiento)
        {
            TipoDeEvento = EnumTipoDeEventoEnJuego.MovimientoJugador;
            DatosDelMovimiento = new MovimientoJugador(usuario, posicionX, posicionY, movimiento);
            Ping = Ping;
        }

        public void EventoEnJuegoIniciarPartida()
        {
            TipoDeEvento = EnumTipoDeEventoEnJuego.IniciarPartida;
            DatosInicioDePartida = new InicioPartida();
            DatosInicioDePartida.IniciarPartida = true;
        }

        public void EventoEnJuegoCambiarPantalla()
        {
            TipoDeEvento = EnumTipoDeEventoEnJuego.CambiarPantalla;
            DatosInicioDePartida = new InicioPartida();
            DatosInicioDePartida.CambiarPantallaMultijugador = true;
        }
    }
    
}