using GameService.Contrato;
using LogicaDelNegocio.Modelo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameService.Dominio.Enum;

namespace GameService.Dominio
{
    public class Sala
    {
        private Dictionary<CuentaModel, IGameServiceCallback> CuentasEnLaSala =
            new Dictionary<CuentaModel, IGameServiceCallback>();

        public String Id { get; set; }

        public int NumeroJugadoresEnSala
        {
            get { return CuentasEnLaSala.Count; }
        }

        private Object ObjetoParaSincronizar = new object();
        public Boolean EsSalaPublica { get; }
        public event NotificacionSalaVacia SalaVacia;

        public delegate void NotificacionSalaVacia(Sala sala);

        public Sala(String id, Boolean EsSalaPublica, CuentaModel cuenta, IGameServiceCallback serviceCallback)
        {
            Id = id;
            CuentasEnLaSala.Add(cuenta, serviceCallback);
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
        public Boolean UnirseASala(CuentaModel cuenta, IGameServiceCallback serviceCallback)
        {
            if (NumeroJugadoresEnSala >= 5) return false;
            lock (ObjetoParaSincronizar)
            {
                Debug.WriteLine("En la sala hay " + CuentasEnLaSala.Count);
                foreach (IGameServiceCallback callback in CuentasEnLaSala.Values)
                {
                    callback.NuevoCuentaEnLaSala((cuenta));
                }
            }

            CuentasEnLaSala.Add(cuenta, serviceCallback);
            if (NumeroJugadoresEnSala != 5) return true;
            {
                lock (ObjetoParaSincronizar)
                {
                    foreach (IGameServiceCallback callback in CuentasEnLaSala.Values)
                    {
                        callback.SalaLlena();
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
                foreach (IGameServiceCallback callback in CuentasEnLaSala.Keys)
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
            VerificarSiLaSalaEstaVacia();
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
    }
}