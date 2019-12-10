using GameService.Contrato;
using LogicaDelNegocio.Modelo;
using System;
using System.Collections.Generic;
using System.Timers;
using LogicaDelNegocio.Modelo.Enum;
using LogicaDelNegocio.DataAccess.Interfaces;
using LogicaDelNegocio.DataAccess;
using System.Diagnostics;
using GameService.Dominio.Enum;
using System.ServiceModel;

namespace GameService.Dominio
{
    /// <summary>
    /// Se encarga de almacenar a cuentas y sus callback permitiendo
    /// </summary>
    public class Sala
    {
        private Dictionary<CuentaModel, IGameServiceCallback> CuentasEnLaSala =
            new Dictionary<CuentaModel, IGameServiceCallback>();
        private Dictionary<CuentaModel, String> DireccionesIpDeCuentas = new Dictionary<CuentaModel, string>();
        private readonly int PUERTO_ENVIO_UDP_1 = 8296;
        private readonly int PUERTO_ENVIO_UDP_2 = 8297;
        private const int CANTIDAD_CUENTAS_MAXIMAS = 4;

        public String Id { get; set; }

        public int NumeroJugadoresEnSala
        {
            get { return CuentasEnLaSala.Count; }
        }

        private Object ObjetoParaSincronizar = new object();
        public Boolean EsSalaPublica { get; }
        public event NotificacionSalaVacia SalaVacia;

        public delegate void NotificacionSalaVacia(Sala sala);

        /// <summary>
        /// Construye una sala con los datos necesarios 
        /// </summary>
        /// <param name="id">String</param>
        /// <param name="EsSalaPublica">Boolean</param>
        /// <param name="cuenta">CuentaModel</param>
        /// <param name="serviceCallback">IGameServiceCallback</param>
        /// <param name="DireccionIpDelCliente">String</param>
        public Sala(String id, Boolean EsSalaPublica, CuentaModel cuenta, IGameServiceCallback serviceCallback, 
            String DireccionIpDelCliente)
        {
            Id = id;
            AsignarTipoDeJugador(cuenta);
            CuentasEnLaSala.Add(cuenta, serviceCallback);
            DireccionesIpDeCuentas.Add(cuenta, DireccionIpDelCliente);
            this.EsSalaPublica = EsSalaPublica;
        }

        /// <summary>
        /// Crea una sala con la información basica
        /// </summary>
        /// <param name="id">String</param>
        /// <param name="EsSalaPublica">Boolean</param>
        public Sala(String id, Boolean EsSalaPublica)
        {
            Id = id;
            this.EsSalaPublica = EsSalaPublica;
        }

        /// <summary>
        /// Agrega una cuenta a la lista de Cuentas en la sala y notifica a las demas cuentas en la sala
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        /// <param name="serviceCallback">IGameServiceCallback</param>
        /// <returns>Verdadero si me pude la cuenta se unio a la sala, falso si no</returns>
        public Boolean UnirseASala(CuentaModel cuenta, IGameServiceCallback serviceCallback, String DireccionIpDelCliente)
        {
            if (NumeroJugadoresEnSala >= 5) return false;
            lock (ObjetoParaSincronizar)
            {
                try
                {
                    AsignarTipoDeJugador(cuenta);
                    foreach (IGameServiceCallback callback in CuentasEnLaSala.Values)
                    {
                        callback.NuevoCuentaEnLaSala(cuenta);
                    }
                    CuentasEnLaSala.Add(cuenta, serviceCallback);
                    DireccionesIpDeCuentas.Add(cuenta, DireccionIpDelCliente);
                }
                catch (ObjectDisposedException)
                {

                }
                catch(TimeoutException)
                {

                }
                catch (CommunicationException)
                {

                }
            }
            return true;
        }

        /// <summary>
        /// Verifica si la sala esta llena
        /// </summary>
        public void VerificarSalaLlena()
        {
            if(NumeroJugadoresEnSala == CANTIDAD_CUENTAS_MAXIMAS)
            {
                lock (ObjetoParaSincronizar)
                {
                    foreach (CuentaModel cuentaEnLaSala in CuentasEnLaSala.Keys)
                    {
                        CuentasEnLaSala[cuentaEnLaSala].SalaLlena();
                    }
                    TemporizarEnvioMensajeCambiarPantalla();
                }
            }
        }

        /// <summary>
        /// Temporiza el envio de mensajes que indica que se inicia el conteo regresivo del inicio de la partida
        /// </summary>
        private void TemporizarEnvioMensajeCambiarPantalla()
        {
            Timer temporizador = new Timer(6000);
            temporizador.Elapsed += EnviarMensajeCambiarDePantalla;
            temporizador.AutoReset = false;
            temporizador.Enabled = true;
            temporizador.Start();
        }

        /// <summary>
        /// Temporiza el envio de mensajes que indica que se inicia el juego
        /// </summary>
        private void TemporizarEnvioMensajeInicioJuego()
        {
            Timer temporizador = new Timer(6000);
            temporizador.Elapsed += EnviarMensajeInicioJuego;
            temporizador.AutoReset = false;
            temporizador.Enabled = true;
            temporizador.Start();
        }

        /// <summary>
        /// Envia a las cuentas de la sala un menasje indicando que inicia la cuenta regresiva
        /// </summary>
        /// <param name="source">object</param>
        /// <param name="e">ElapsedEventArgs</param>
        private void EnviarMensajeCambiarDePantalla(object source, ElapsedEventArgs e)
        {
            EventoEnJuego eventoEnJuego = new EventoEnJuego();
            eventoEnJuego.EventoEnJuegoCambiarPantalla(Id);
            foreach (String direccionIp in DireccionesIpDeCuentas.Values)
            {
                UdpSender EnviadorDePaquetes = new UdpSender(direccionIp, PUERTO_ENVIO_UDP_1, PUERTO_ENVIO_UDP_2);
                EnviadorDePaquetes.EnviarPaquete(eventoEnJuego);
            }
            TemporizarEnvioMensajeInicioJuego();
        }

        /// <summary>
        /// Envia un mensaje a las cuentas de la sala indicando que el juego esta por comenzar
        /// </summary>
        /// <param name="source">object</param>
        /// <param name="e">ElapsedEventArgs</param>
        private void EnviarMensajeInicioJuego(object source, ElapsedEventArgs e)
        {
            EventoEnJuego eventoEnJuego = new EventoEnJuego();
            eventoEnJuego.EventoEnJuegoIniciarPartida(Id);
            foreach (String direccionIp in DireccionesIpDeCuentas.Values)
            {
                UdpSender EnviadorDePaquetes = new UdpSender(direccionIp, PUERTO_ENVIO_UDP_1, PUERTO_ENVIO_UDP_2);
                EnviadorDePaquetes.EnviarPaquete(eventoEnJuego);
            }
        }

        /// <summary>
        /// Notifica a las cuentas de la sala que la partida ha terminado
        /// </summary>
        void TerminarPartida()
        {
            lock (ObjetoParaSincronizar)
            {
                try
                {
                    foreach (IGameServiceCallback callback in CuentasEnLaSala.Values)
                    {
                        callback.NotificarTerminaPartida();
                    }
                }
                catch (ObjectDisposedException)
                {

                }
                catch (CommunicationException)
                {

                }
                catch (TimeoutException)
                {

                }
                
            }
        }

        /// <summary>
        /// Busca si la cuenta esta en la sala
        /// </summary>
        /// <param name="cuentaABuscar">CuentaModel</param>
        /// <returns>Verdadero si la sala se encuentra en la sala, falso si no</returns>
        public Boolean EstaLaCuentaEnLaSala(CuentaModel cuentaABuscar)
        {
            Boolean EstaEnLaSala = false;
            foreach (CuentaModel cuenta in CuentasEnLaSala.Keys)
            {
                if (cuenta.NombreUsuario == cuentaABuscar.NombreUsuario)
                {
                    EstaEnLaSala = true;
                }
            }
            return EstaEnLaSala;
        }

        /// <summary>
        /// Verifica si la sala tiene 5 jugadores
        /// </summary>
        /// <returns>Verdadero si esta la sala llena, falso si no</returns>
        public Boolean EstaLaSalaLlena()
        {
            return NumeroJugadoresEnSala >= CANTIDAD_CUENTAS_MAXIMAS;
        }

        /// <summary>
        /// Regresa las cuentas que se encuentran en la sala
        /// </summary>
        /// <returns>Una lista de cuentas con las cuentas que se encuentran en las salas</returns>
        public List<CuentaModel> RecuperarCuentasEnLaSala()
        {
            List<CuentaModel> CuentasEnSala = new List<CuentaModel>();
            foreach (CuentaModel cuenta in CuentasEnLaSala.Keys)
            {
                CuentasEnSala.Add(cuenta);
            }

            return CuentasEnSala;
        }

        /// <summary>
        /// Elimina a la cuenta de las cuentas en la sala y notifica a las demas cuentas
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        public void AbandonarSala(CuentaModel cuenta)
        {
            CuentaModel cuentaAEliminar = null;
            foreach (CuentaModel cuentaEnLaSala in CuentasEnLaSala.Keys)
            {
                if (cuentaEnLaSala.NombreUsuario == cuenta.NombreUsuario)
                {
                    cuentaAEliminar = cuentaEnLaSala;
                }
            }
            CuentasEnLaSala.Remove(cuentaAEliminar);
            DireccionesIpDeCuentas.Remove(cuentaAEliminar);
            VerificarSiLaSalaEstaVacia();
            ReasignarTipoDeJugador();
            NotificarCuentasEnSalaSalidaDeJugador(cuentaAEliminar);
        }

        /// <summary>
        /// Notifica a las cuentas de la sala que una cuenta ha salido
        /// </summary>
        /// <param name="CuentaAEliminar">CuentaModel</param>
        private void NotificarCuentasEnSalaSalidaDeJugador(CuentaModel CuentaAEliminar)
        {
            lock (ObjetoParaSincronizar)
            {
                List<CuentaModel> cuentasEnLaSala = RecuperarCuentasEnLaSala();
                foreach (IGameServiceCallback canal in CuentasEnLaSala.Values)
                {
                    try
                    {
                        canal.CuentaAbandoSala(CuentaAEliminar);
                        canal.RefrescarCuentasEnSala(cuentasEnLaSala);
                    }
                    catch (ObjectDisposedException)
                    {
                        //Preguntar
                    }
                    catch (CommunicationObjectAbortedException)
                    {

                    }
                    catch (TimeoutException)
                    {

                    }
                }
            }
        }
        
        /// <summary>
        /// Verifica sila sala esta vacia, y si lo esta notifica
        /// </summary>
        private void VerificarSiLaSalaEstaVacia()
        {
            if (NumeroJugadoresEnSala == 0)
            {
                SalaVacia?.Invoke(this);
            }
        }

        /// <summary>
        /// Establece el tipo de jugador de la cuenta en la sala
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        private void AsignarTipoDeJugador(CuentaModel cuenta)
        {
            cuenta.Jugador.RolDelJugador = (EnumTipoDeJugador) NumeroJugadoresEnSala;
        }

        /// <summary>
        /// Reasigna el tipo de jugador de las cuentas en la sala
        /// </summary>
        private void ReasignarTipoDeJugador()
        {
            int contadorDeCuentasEnLaSala = 0;
            foreach (CuentaModel cuentaEnSala in CuentasEnLaSala.Keys)
            {
                cuentaEnSala.Jugador.RolDelJugador = (EnumTipoDeJugador) contadorDeCuentasEnLaSala;
                contadorDeCuentasEnLaSala++;
            }
        }

        /// <summary>
        /// Replica un mensaje recibido de una cuenta a las demas cuentas de las sala
        /// </summary>
        /// <param name="eventoEnJuego">EventoEnJuego</param>
        public void ReplicarMensajeACuentas(EventoEnJuego eventoEnJuego)
        {
            foreach (String DireccionIp in DireccionesIpDeCuentas.Values)
            {
                UdpSender EnviadorDePaquetesUDP = new UdpSender(DireccionIp, PUERTO_ENVIO_UDP_1, PUERTO_ENVIO_UDP_2);
                EnviadorDePaquetesUDP.EnviarPaquete(eventoEnJuego);
            }
        }

        /// <summary>
        /// Notifica a las cuentas que la partida a terminado y almacena los datos del corredor
        /// </summary>
        /// <param name="CuentaDelCorredor">CuentaModel</param>
        /// <returns>EnumEstadoTerminarPartida</returns>
        public EnumEstadoTerminarPartida TerminarPartida(CuentaModel CuentaDelCorredor)
        {
            foreach(IGameServiceCallback callback in CuentasEnLaSala.Values)
            {
                callback.NotificarTerminaPartida();
            }
            ICuentaDAO PersistenciaDeCuenta = new CuentaDAO();
            try
            {
                PersistenciaDeCuenta.GuardarDatosDeLaCuenta(CuentaDelCorredor);
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.InnerException.Message);
            }
            return EnumEstadoTerminarPartida.TerminadaCorrectamente;
        }

        /// <summary>
        /// Notifica a las cuentas en la sesion que se iniciara un nuevo nivel
        /// </summary>
        public void NotificarIniciarNivel()
        {
            try
            {
                foreach (IGameServiceCallback callback in CuentasEnLaSala.Values)
                {
                    callback.NuevoNivel();
                }
            }
            catch (TimeoutException)
            {

            }
            catch (ObjectDisposedException)
            {

            }
            catch (CommunicationException)
            {

            }
            
            TemporizarEnvioMensajeCuentaRegresivaNuevoNivel();
        }

        /// <summary>
        /// Temporiza el envio de un mensaje de iniciarCuenta
        /// </summary>
        private void TemporizarEnvioMensajeCuentaRegresivaNuevoNivel()
        {
            Timer temporizador = new Timer(6000);
            temporizador.Elapsed += EnviarMensajeInicioCuentaRegresivaNuevoNivel;
            temporizador.AutoReset = false;
            temporizador.Enabled = true;
            temporizador.Start();
        }

        /// <summary>
        /// Temporiza el envio de mensaje de inicio de nivel
        /// </summary>
        private void TemporizarEnvioMensajeInicioNivel()
        {
            Timer temporizador = new Timer(6000);
            temporizador.Elapsed += EnviarMensajeInicioJuego;
            temporizador.AutoReset = false;
            temporizador.Enabled = true;
            temporizador.Start();
        }

        /// <summary>
        /// Envia un mensaje de inicio de cuenta regresiva de nuevo nivel
        /// </summary>
        /// <param name="source">object</param>
        /// <param name="e">ElapsedEventAergs</param>
        private void EnviarMensajeInicioCuentaRegresivaNuevoNivel(object source, ElapsedEventArgs e)
        {
            EventoEnJuego eventoEnJuego = new EventoEnJuego();
            eventoEnJuego.EventoEnJuegoIniciarConteoNuevoNivel(Id);
            foreach (String direccionIp in DireccionesIpDeCuentas.Values)
            {
                UdpSender EnviadorDePaquetes = new UdpSender(direccionIp, PUERTO_ENVIO_UDP_1, PUERTO_ENVIO_UDP_2);
                EnviadorDePaquetes.EnviarPaquete(eventoEnJuego);
            }
            TemporizarEnvioMensajeInicioNivel();
        }

        /// <summary>
        /// Envia un mensaje de inicio de nivel a las cuentas en la sala
        /// </summary>
        /// <param name="source">object</param>
        /// <param name="e">ElapsedEventArgs</param>
        private void EnviarMensajeInicioNivel(object source, ElapsedEventArgs e)
        {
            EventoEnJuego eventoEnJuego = new EventoEnJuego();
            eventoEnJuego.EventoEnJuegoIniciarNuevoNivel(Id);
            foreach (String direccionIp in DireccionesIpDeCuentas.Values)
            {
                UdpSender EnviadorDePaquetes = new UdpSender(direccionIp, PUERTO_ENVIO_UDP_1, PUERTO_ENVIO_UDP_2);
                EnviadorDePaquetes.EnviarPaquete(eventoEnJuego);
            }
        }
    }
}