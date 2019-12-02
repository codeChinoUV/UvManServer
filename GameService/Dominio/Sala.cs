using GameService.Contrato;
using LogicaDelNegocio.Modelo;
using System;
using System.Collections.Generic;
using GameService.Dominio.Enum;

namespace GameService.Dominio
{
    public class Sala
    {
        private Dictionary<CuentaModel, IGameServiceCallback> CuentasEnLaSala =
            new Dictionary<CuentaModel, IGameServiceCallback>();
        private Dictionary<CuentaModel, String> DireccionesIpDeCuentas = new Dictionary<CuentaModel, string>();

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
            }
            CuentasEnLaSala.Add(cuenta, serviceCallback);
            DireccionesIpDeCuentas.Add(cuenta, DireccionIpDelCliente);
            if (NumeroJugadoresEnSala != 5) return true;
            {
                lock (ObjetoParaSincronizar)
                {
                    EventoEnJuego eventoEnJuego = new EventoEnJuego();
                    eventoEnJuego.EventoEnJuegoIniciarPartida();
                    foreach (IGameServiceCallback callback in CuentasEnLaSala.Values)
                    {
                        callback.SalaLlena();
                    }
                    foreach(String direccionIp in DireccionesIpDeCuentas.Values)
                    {
                        UdpSender EnviadorDePaquetes = new UdpSender(direccionIp);
                        EnviadorDePaquetes.EnviarPaquete(eventoEnJuego);
                    }
                    
                }
            }
            return true;
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
                    callback.TerminarPartida();
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
                canal.CuentaAbandoSala(CuentaAEliminar);
                canal.RefrescarCuentasEnSala(cuentasEnLaSala);
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
                UdpSender EnviadorDePaquetesUDP = new UdpSender(DireccionIp);
                EnviadorDePaquetesUDP.EnviarPaquete(eventoEnJuego);
            }
        }

        
    }
}