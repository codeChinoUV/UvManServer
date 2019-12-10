using GameService.Contrato;
using GameService.Dominio.Enum;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.Util;
using System;
using System.Collections.Generic;

namespace GameService.Dominio
{
    /// <summary>
    /// Se encarga de administrar la salas
    /// </summary>
    public sealed class SalaManager
    {
        private static SalaManager ManejadorDeSala = new SalaManager();
        private static List<Sala> SalasCreadas = new List<Sala>();
        private SessionManager ManejadorDeSesiones = SessionManager.GetSessionManager();

        public delegate void NotificacionSobreSala(Sala salaCreada);
        public event NotificacionSobreSala SalaCreada;
        public event NotificacionSobreSala SalaDestriuda;

        public delegate void NotificacionSobreCuentaEnSala(String IdSala);
        public event NotificacionSobreCuentaEnSala SeUnioASala;
        public event NotificacionSobreCuentaEnSala DejoSala;

        private SalaManager()
        {
            SuscribirseAEventosDeSesion();
        }

        /// <summary>
        /// Se suscribe a los eventos que ofrece la sala
        /// </summary>
        /// <param name="salaASuscribirse">Sala</param>
        private void SuscribirseAEventosDeSala(Sala salaASuscribirse)
        {
            salaASuscribirse.SalaVacia += DestruirSala;
        }

        /// <summary>
        /// Destruye una sala y envia una notificacion
        /// </summary>
        /// <param name="salaADestruir">Sala</param>
        private void DestruirSala(Sala salaADestruir)
        {
            SalasCreadas.Remove(salaADestruir);
            SalaDestriuda?.Invoke(salaADestruir);
        }

        /// <summary>
        /// Se suscribe a los eventos que ofrece el SessionManager
        /// </summary>
        private void SuscribirseAEventosDeSesion()
        {
            ManejadorDeSesiones.UsuarioDesconectado += AbandonarSala;
        }

        /// <summary>
        /// Regresa la instacia de la clase SalaManager
        /// </summary>
        /// <returns>SalaManager</returns>
        public static SalaManager GetSalaManager()
        {
            return ManejadorDeSala;
        }

        /// <summary>
        /// Agrega una Cuenta al a la sala que tenga ese Id 
        /// </summary>
        /// <param name="Id">String</param>
        /// <param name="CuentaAAgregar">Cuenta</param>
        /// <param name="ActualCallback">IGameServiceCallback</param>
        /// <returns>EnumEstadoDeUnirseASala</returns>
        public EnumEstadoDeUnirseASala UnirseASalaConId(String Id, CuentaModel CuentaAAgregar, IGameServiceCallback ActualCallback,
            String DireccionIpDelCliente)
        {
            EnumEstadoDeUnirseASala estadoDeUnirseASala = EnumEstadoDeUnirseASala.NoSeEncuentraEnSesion;
            if (ManejadorDeSesiones.VerificarCuentaLogeada(CuentaAAgregar))
            {
                estadoDeUnirseASala = EnumEstadoDeUnirseASala.SalaInexistente;
                CuentaAAgregar = ManejadorDeSesiones.ObtenerCuentaCompleta(CuentaAAgregar);
                foreach (Sala sala in SalasCreadas)
                {
                    if (sala.Id == Id)
                    {
                        if (sala.UnirseASala(CuentaAAgregar, ActualCallback, DireccionIpDelCliente))
                        {
                            estadoDeUnirseASala = EnumEstadoDeUnirseASala.UnidoCorrectamente;
                            SeUnioASala?.Invoke(sala.Id);
                        }
                        else
                        {
                            estadoDeUnirseASala = EnumEstadoDeUnirseASala.SalaLlena;
                        }
                        break;
                    }
                }
            }
            
            return estadoDeUnirseASala;
        }

        /// <summary>
        /// Agrega una Cuenta a una sala completa, si no hay salas disponibles la crea
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <param name="ActualCallback">IGameServiceCallback</param>
        /// <returns>Verdadero si la sala cuenta se unico correctamenete a la sala o falso si no</returns>
        public Boolean UnisrseASalaDisponible(CuentaModel Cuenta, IGameServiceCallback ActualCallback, String DireccionIpDelCliente)
        {
            Boolean seUnioASala = false;
            if (ManejadorDeSesiones.VerificarCuentaLogeada(Cuenta))
            {
                Cuenta = ManejadorDeSesiones.ObtenerCuentaCompleta(Cuenta);
                Sala SalaAUnirse = BuscarSalaIncompleta();
                if (SalaAUnirse != null)
                {
                    seUnioASala = SalaAUnirse.UnirseASala(Cuenta, ActualCallback, DireccionIpDelCliente);
                    SeUnioASala?.Invoke(SalaAUnirse.Id);
                }
                else
                {
                    Sala NuevaSala = CrearSalaConIdAleatorio(Cuenta, ActualCallback, DireccionIpDelCliente);
                    SalasCreadas.Add(NuevaSala);
                    SuscribirseAEventosDeSala(NuevaSala);
                    SalaCreada?.Invoke(NuevaSala);
                    seUnioASala = true;
                }                
            }
            return seUnioASala;
        }

        /// <summary>
        /// Crea una Sala personalizada
        /// </summary>
        /// <param name="Id">String</param>
        /// <param name="EsSalaPublica">Boolean</param>
        /// <param name="Cuenta">CuentaModel</param>
        /// <param name="ActualCallback">IGameServiceCallback</param>
        /// <returns>EnumEstadoCrearSalaConId</returns>
        public EnumEstadoCrearSalaConId CrearSala(string Id, Boolean EsSalaPublica, CuentaModel Cuenta, 
            IGameServiceCallback ActualCallback, String DireccionIpDelCliente)
        {
            EnumEstadoCrearSalaConId EstadoDeCreacionDeSala = EnumEstadoCrearSalaConId.NoSeEncuentraEnSesion;
            if (ManejadorDeSesiones.VerificarCuentaLogeada(Cuenta))
            {
                Cuenta = ManejadorDeSesiones.ObtenerCuentaCompleta(Cuenta);
                if (!EstaElIdDeSalaEnUso(Id))
                {
                    Sala SalaAAgregar = new Sala(Id, EsSalaPublica, Cuenta, ActualCallback, DireccionIpDelCliente);
                    SalasCreadas.Add(SalaAAgregar);
                    SuscribirseAEventosDeSala(SalaAAgregar);
                    SalaCreada?.Invoke(SalaAAgregar);
                    EstadoDeCreacionDeSala = EnumEstadoCrearSalaConId.CreadaCorrectamente;
                }
                else
                {
                    EstadoDeCreacionDeSala = EnumEstadoCrearSalaConId.IdYaExistente;
                }
            }
            
            return EstadoDeCreacionDeSala;
        }

        /// <summary>
        /// Verifica si la cuenta se encuentra en una sala
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>Verdadero si la cuenta se encuentra en la sala, falso si no</returns>
        public bool VerificarSiEstoyEnSala(CuentaModel Cuenta)
        {
            Boolean SeEncuentraEnSala = false;
            foreach (Sala sala in SalasCreadas)
            {
                if (sala.EstaLaCuentaEnLaSala(Cuenta))
                {
                    SeEncuentraEnSala = true;
                }
            }
            return SeEncuentraEnSala;
        }

        /// <summary>
        /// Recupera las cuentas que estan en la sala de la cuenta
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>Una lista con las cuentas de la sala del jugagor</returns>
        public List<CuentaModel> RecuperarCuentasDeSalaDeJugador(CuentaModel Cuenta)
        {
            List<CuentaModel> CuentasDeLaSala;
            Sala SalaDondeSeEncuentraLaCuenta = RecuperarSalaDeCuenta(Cuenta);
            if(SalaDondeSeEncuentraLaCuenta != null)
            {
                CuentasDeLaSala = SalaDondeSeEncuentraLaCuenta.RecuperarCuentasEnLaSala();
            }
            else
            {
                CuentasDeLaSala = new List<CuentaModel>();            
            }
            return CuentasDeLaSala;
        }

        /// <summary>
        /// Recupera la sala en la que se encuentra la cuenta
        /// </summary>
        /// <param name="Cuenta">Cuenta</param>
        /// <returns>La sala de la cuenta</returns>
        public Sala RecuperarSalaDeCuenta(CuentaModel Cuenta)
        {
            Sala SalaDondeSeEncuentraLaCuenta = null;
            foreach(Sala sala in SalasCreadas)
            {
                if (sala.EstaLaCuentaEnLaSala(Cuenta))
                {
                    SalaDondeSeEncuentraLaCuenta = sala;
                }
            }
            return SalaDondeSeEncuentraLaCuenta;
        }
    
        /// <summary>
        /// Busca la sala incompleta con mas cuentas
        /// </summary>
        /// <returns>La sala que aun no esta completa</returns>
        private Sala BuscarSalaIncompleta()
        {
            Sala SalaConMayorNumeroDeJugadores = null;
            foreach (Sala sala in SalasCreadas)
            {
                if (sala.EsSalaPublica && !sala.EstaLaSalaLlena())
                {
                    if(SalaConMayorNumeroDeJugadores == null)
                    {
                        SalaConMayorNumeroDeJugadores = sala;
                    }else
                    {
                        if (SalaConMayorNumeroDeJugadores.NumeroJugadoresEnSala < sala.NumeroJugadoresEnSala)
                        {
                            SalaConMayorNumeroDeJugadores = sala;
                        }
                    }
                }
            }
            return SalaConMayorNumeroDeJugadores;
        }

        /// <summary>
        /// Busca si el Id ya se encuentra ocupado por una sala
        /// </summary>
        /// <param name="idAComparar">String</param>
        /// <returns>Verdadero si el Id ya se encuentra en uso por otra sala y falso si no</returns>
        private Boolean EstaElIdDeSalaEnUso(String idAComparar)
        {
            Boolean IdEstaOcupado = false;
            foreach (Sala sala in SalasCreadas)
            {
                if (sala.Id == idAComparar)
                {
                    IdEstaOcupado = true;
                }
            }
            return IdEstaOcupado;
        }

        /// <summary>
        /// Crea una sala publica con Id aleatorio
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <param name="ActualCallback">IGameServiceCallback</param>
        /// <returns>La sala que se creada</returns>
        private Sala CrearSalaConIdAleatorio(CuentaModel Cuenta, IGameServiceCallback ActualCallback, String DireccionIpDelCliente)
        {
            String IdDeSala;
            do
            {
                IdDeSala = GeneradorCodigo.GenerarCadena();
            } while (EstaElIdDeSalaEnUso(IdDeSala));
            Sala NuevaSala = new Sala(IdDeSala, true, Cuenta, ActualCallback, DireccionIpDelCliente);
            return NuevaSala;
        }

        /// <summary>
        /// Saca de la sala a la cuenta
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        public void AbandonarSala(CuentaModel cuenta)
        {
            Sala SalaDeLaCuenta = RecuperarSalaDeCuenta(cuenta);
            if(SalaDeLaCuenta != null)
            {
                SalaDeLaCuenta.AbandonarSala(cuenta);
                DejoSala?.Invoke(SalaDeLaCuenta.Id) ;
            }
        }
        
        /// <summary>
        /// Busca la sala a la cual se le deben de reenviar los datos y se los reenvia
        /// </summary>
        /// <param name="eventoEnJuego">EventoEnJuego</param>
        public void ReplicarDatosRecibidosASala(EventoEnJuego eventoEnJuego)
        {
            foreach (Sala sala in SalasCreadas)
            {
                if (sala.Id == eventoEnJuego.IdSala)
                {
                    sala.ReplicarMensajeACuentas(eventoEnJuego);
                    break;
                }
            }
        }

        /// <summary>
        /// Recupera la cuenta la cual envio el EventoEnJuego
        /// </summary>
        /// <param name="eventoEnJuego">EventoEnJuego</param>
        /// <returns>La cuenta que envio el evento </returns>
        private CuentaModel RecuperarCuentaDelEvento(EventoEnJuego eventoEnJuego)
        {
            CuentaModel cuentaDelEvento = new CuentaModel();
            switch (eventoEnJuego.TipoDeEvento)
            {
                case EnumTipoDeEventoEnJuego.MovimientoJugador:
                    cuentaDelEvento.NombreUsuario = eventoEnJuego.DatosDelMovimiento.Usuario;
                    break;
                case EnumTipoDeEventoEnJuego.MuerteJugador:
                    cuentaDelEvento.NombreUsuario = eventoEnJuego.DatosMuerteDeUnJugador.Usuario;
                    break;
            }

            return cuentaDelEvento;
        }

        /// <summary>
        /// Le indica a la sala de la CuentaDelCorredor que se envio un mensaje de terminar partida
        /// </summary>
        /// <param name="CuentaDelCorredor">CuentaModel</param>
        public void TerminarPartida(CuentaModel CuentaDelCorredor)
        {
            Sala MiSala = ManejadorDeSala.RecuperarSalaDeCuenta(CuentaDelCorredor);
            if (MiSala != null)
            {
                MiSala.TerminarPartida(CuentaDelCorredor);
            }
        }

        public void IniciarNivel(CuentaModel CuentaDelCorredor)
        {
            Sala MiSala = ManejadorDeSala.RecuperarSalaDeCuenta(CuentaDelCorredor);
            if (MiSala != null)
            {
                MiSala.NotificarIniciarNivel();
            }
        }
    }
}
