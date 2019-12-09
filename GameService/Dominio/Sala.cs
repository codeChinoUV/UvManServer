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
    public class Sala
    {
        private Dictionary<CuentaModel, IGameServiceCallback> CuentasEnLaSala =
            new Dictionary<CuentaModel, IGameServiceCallback>();
        private Dictionary<CuentaModel, String> DireccionesIpDeCuentas = new Dictionary<CuentaModel, string>();
        private readonly int PUERTO_ENVIO_UDP_1 = 8296;
        private readonly int PUERTO_ENVIO_UDP_2 = 8297;

        public String Id { get; set; }

        public int NumeroJugadoresEnSala
        {
            get { return CuentasEnLaSala.Count; }
        }

        private Object ObjetoParaSincronizar = new object();
        public Boolean EsSalaPublica { get; }
        public event NotificacionSalaVacia SalaVacia;

        public delegate void NotificacionSalaVacia(Sala sala);

        public Sala(String id, Boolean EsSalaPublica, CuentaModel cuenta, IGameServiceCallback serviceCallback, 
            String DireccionIpDelCliente)
        {
            Id = id;
            AsignarTipoDeJugador(cuenta);
            CuentasEnLaSala.Add(cuenta, serviceCallback);
            DireccionesIpDeCuentas.Add(cuenta, DireccionIpDelCliente);
            this.EsSalaPublica = EsSalaPublica;
        }

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
        /// <returns>Boolean</returns>
        public Boolean UnirseASala(CuentaModel cuenta, IGameServiceCallback serviceCallback, String DireccionIpDelCliente)
        {
            if (NumeroJugadoresEnSala >= 5) return false;
            lock (ObjetoParaSincronizar)
            {
                AsignarTipoDeJugador(cuenta);
                foreach (IGameServiceCallback callback in CuentasEnLaSala.Values)
                {
                    callback.NuevoCuentaEnLaSala(cuenta);
                }
                CuentasEnLaSala.Add(cuenta, serviceCallback);
                DireccionesIpDeCuentas.Add(cuenta, DireccionIpDelCliente);
            }
            return true;
        }

        public void VerificarSalaLlena()
        {
            if(NumeroJugadoresEnSala == 2)
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

        private void TemporizarEnvioMensajeCambiarPantalla()
        {
            Timer temporizador = new Timer(6000);
            temporizador.Elapsed += EnviarMensajeCambiarDePantalla;
            temporizador.AutoReset = false;
            temporizador.Enabled = true;
            temporizador.Start();
        }

        private void TemporizarEnvioMensajeInicioJuego()
        {
            Timer temporizador = new Timer(6000);
            temporizador.Elapsed += EnviarMensajeInicioJuego;
            temporizador.AutoReset = false;
            temporizador.Enabled = true;
            temporizador.Start();
        }

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
                foreach (IGameServiceCallback callback in CuentasEnLaSala.Values)
                {
                    callback.NotificarTerminaPartida();
                }
            }
        }

        /// <summary>
        /// Busca si la cuenta esta en la sala
        /// </summary>
        /// <param name="cuentaABuscar">CuentaModel</param>
        /// <returns>Boolean</returns>
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
        /// <returns>Boolean</returns>
        public Boolean EstaLaSalaLlena()
        {
            return NumeroJugadoresEnSala >= 5;
        }

        /// <summary>
        /// Regresa las cuentas que se encuentran en la sala
        /// </summary>
        /// <returns>List</returns>
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
        /// <param name="cuenta"></param>
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

        public void ReplicarMensajeACuentas(EventoEnJuego eventoEnJuego)
        {
            foreach (String DireccionIp in DireccionesIpDeCuentas.Values)
            {
                UdpSender EnviadorDePaquetesUDP = new UdpSender(DireccionIp, PUERTO_ENVIO_UDP_1, PUERTO_ENVIO_UDP_2);
                EnviadorDePaquetesUDP.EnviarPaquete(eventoEnJuego);
            }
        }

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

        public void NotificarIniciarNivel()
        {
            foreach(IGameServiceCallback callback in CuentasEnLaSala.Values)
            {
                callback.NuevoNivel();
            }
            TemporizarEnvioMensajeCuentaRegresivaNuevoNivel();
        }

        private void TemporizarEnvioMensajeCuentaRegresivaNuevoNivel()
        {
            Timer temporizador = new Timer(6000);
            temporizador.Elapsed += EnviarMensajeInicioCuentaRegresivaNuevoNivel;
            temporizador.AutoReset = false;
            temporizador.Enabled = true;
            temporizador.Start();
        }

        private void TemporizarEnvioMensajeInicioNivel()
        {
            Timer temporizador = new Timer(6000);
            temporizador.Elapsed += EnviarMensajeInicioJuego;
            temporizador.AutoReset = false;
            temporizador.Enabled = true;
            temporizador.Start();
        }

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