using System;
using GameService.Dominio.Enum;

namespace GameService.Dominio
{
    [Serializable]
    public class EventoEnJuego
    {
        public String IdSala { get; set; }
        
        public  String CuentaRemitente { get; set; }
        public EnumTipoDeEventoEnJuego TipoDeEvento { get; set; }
       
        public MovimientoJugador DatosDelMovimiento { get; set; }
        
        public MuerteJugador DatosMuerteDeUnJugador { get; set; }
        
        public InicioPartida DatosInicioDePartida { set; get; }
        
        public int Ping { get; set; }

        public void EventoEnJuegoMuerteJugador(String cuentaOrigen, String idSala, String usuarioMuerto,
            int cantidadDeVidas, int ping)
        {
            CuentaRemitente = cuentaOrigen;
            IdSala = idSala;
            TipoDeEvento = EnumTipoDeEventoEnJuego.MuerteJugador;
            DatosMuerteDeUnJugador = new MuerteJugador(usuarioMuerto, cantidadDeVidas);
            Ping = ping;
        }

        public void EventoEnJuegoMovimientoJugador(String cuentaOrigen, String sala, String usuario, 
            float posicionX, float posicionY, float movimientoX, float movimientoY)
        {
            CuentaRemitente = cuentaOrigen;
            IdSala = sala;
            TipoDeEvento = EnumTipoDeEventoEnJuego.MovimientoJugador;
            DatosDelMovimiento = new MovimientoJugador(usuario, posicionX, posicionY, movimientoX, movimientoY);
            Ping = Ping;
        }

        public void EventoEnJuegoIniciarPartida(String sala)
        {
            IdSala = sala;
            TipoDeEvento = EnumTipoDeEventoEnJuego.IniciarPartida;
            DatosInicioDePartida = new InicioPartida();
            DatosInicioDePartida.IniciarPartida = true;
        }

        public void EventoEnJuegoCambiarPantalla(String sala)
        {
            IdSala = sala;
            TipoDeEvento = EnumTipoDeEventoEnJuego.CambiarPantalla;
            DatosInicioDePartida = new InicioPartida();
            DatosInicioDePartida.CambiarPantallaMultijugador = true;
        }
    }
    
}