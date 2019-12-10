using System;
using GameService.Dominio.Enum;

namespace GameService.Dominio
{
    /// <summary>
    /// Se encarga de encapsular mensajes de eventos del juego que seran enviados atraves de Udp
    /// </summary>
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

        /// <summary>
        /// Crea un evento del juego indicando la muerte de un jugador
        /// </summary>
        /// <param name="cuentaOrigen">String</param>
        /// <param name="idSala">String</param>
        /// <param name="usuarioMuerto">String</param>
        /// <param name="cantidadDeVidas">String</param>
        /// <param name="ping">int</param>
        public void EventoEnJuegoMuerteJugador(String cuentaOrigen, String idSala, String usuarioMuerto,
            int cantidadDeVidas, int ping)
        {
            CuentaRemitente = cuentaOrigen;
            IdSala = idSala;
            TipoDeEvento = EnumTipoDeEventoEnJuego.MuerteJugador;
            DatosMuerteDeUnJugador = new MuerteJugador(usuarioMuerto, cantidadDeVidas);
            Ping = ping;
        }

        /// <summary>
        /// Crea un evento del juego indicando el movimiento de un jugador
        /// </summary>
        /// <param name="cuentaOrigen">String</param>
        /// <param name="sala">String</param>
        /// <param name="usuario">String</param>
        /// <param name="posicionX">int</param>
        /// <param name="posicionY">int</param>
        /// <param name="movimientoX">int</param>
        /// <param name="movimientoY">int</param>
        public void EventoEnJuegoMovimientoJugador(String cuentaOrigen, String sala, String usuario, 
            float posicionX, float posicionY, float movimientoX, float movimientoY)
        {
            CuentaRemitente = cuentaOrigen;
            IdSala = sala;
            TipoDeEvento = EnumTipoDeEventoEnJuego.MovimientoJugador;
            DatosDelMovimiento = new MovimientoJugador(usuario, posicionX, posicionY, movimientoX, movimientoY);
            Ping = Ping;
        }

        /// <summary>
        /// Crea un evento del juego de tipo iniciar partida con sus datos
        /// </summary>
        /// <param name="sala">String</param>
        public void EventoEnJuegoIniciarPartida(String sala)
        {
            IdSala = sala;
            TipoDeEvento = EnumTipoDeEventoEnJuego.IniciarPartida;
            DatosInicioDePartida = new InicioPartida
            {
                IniciarPartida = true
            };
        }

        /// <summary>
        /// Crea un evento del juego de tipo Cambiar pantalla con sus datos
        /// </summary>
        /// <param name="sala">Stirng</param>
        public void EventoEnJuegoCambiarPantalla(String sala)
        {
            IdSala = sala;
            TipoDeEvento = EnumTipoDeEventoEnJuego.IniciarCuentaRegresivaInicioJuego;
            DatosInicioDePartida = new InicioPartida();
            DatosInicioDePartida.IniciarCuentaRegresivaInicioPartida = true;
        }

        /// <summary>
        /// Crea un evento del juego de tipo inicar conteo de nivel con sus datos
        /// </summary>
        /// <param name="sala">String</param>
        public void EventoEnJuegoIniciarConteoNuevoNivel(String sala)
        {
            IdSala = sala;
            TipoDeEvento = EnumTipoDeEventoEnJuego.IniciarCuentaRegresivaInicioNivel;
            DatosInicioDePartida = new InicioPartida();
            DatosInicioDePartida.IniciarCuentaRegresivaInicioNivel = true;
        }

        /// <summary>
        /// Crea un evento del juego de tipo inicar conteo de nivel con sus datos
        /// </summary>
        /// <param name="sala">String</param>
        public void EventoEnJuegoIniciarNuevoNivel(String sala)
        {
            IdSala = sala;
            TipoDeEvento = EnumTipoDeEventoEnJuego.IniciarNivel;
            DatosInicioDePartida = new InicioPartida();
            DatosInicioDePartida.IniciarCuentaRegresivaInicioNivel = true;
        }
    }
    
}