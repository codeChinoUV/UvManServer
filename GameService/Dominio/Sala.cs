using GameService.Contrato;
using LogicaDelNegocio.Modelo;
using System;
using System.Collections.Generic;
namespace GameService.Dominio
{
    public class Sala
    {
        private Dictionary<CuentaModel, IGameServiceCallback> CuentasEnLaSala = new Dictionary<CuentaModel, IGameServiceCallback>();
        public String Id;
        int NumeroJugadoresEnSala = 0;
        private Object ObjetoParaSincronizar = new object();
        public Boolean EsSalaPublica { get; }

        public Sala(String id, Boolean EsSalaPublica, CuentaModel cuenta, IGameServiceCallback serviceCallback)
        {
            Id = id;
            CuentasEnLaSala.Add(cuenta, serviceCallback);
            NumeroJugadoresEnSala += 1;
            this.EsSalaPublica = EsSalaPublica;
        }

        public Sala(String id, Boolean EsSalaPublica)
        {
            Id = id;
            this.EsSalaPublica = EsSalaPublica;
        }

        public Sala()
        {

        }

        public Boolean UnirseASala(CuentaModel cuenta, IGameServiceCallback serviceCallback)
        {
            if(NumeroJugadoresEnSala < 5)
            {
                CuentasEnLaSala.Add(cuenta, serviceCallback);
                NumeroJugadoresEnSala += 1;
                serviceCallback.NuevoCuentaEnLaSala(cuenta);
                if(NumeroJugadoresEnSala == 5)
                {
                    serviceCallback.SalaLlena();
                }
                return true;
            }
            return false;
        }

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

        public Boolean EstaLaCuentaEnLaSala(CuentaModel cuentaABuscar)
        {
            Boolean EstaEnLaSala = false;
            foreach(CuentaModel cuenta in CuentasEnLaSala.Keys)
            {
                if(cuenta.NombreUsuario == cuentaABuscar.NombreUsuario)
                {
                    EstaEnLaSala = true;
                }
            }
            return EstaEnLaSala;
        }

        public Boolean EstaLaSalaLlena()
        {
            return NumeroJugadoresEnSala >= 5;
        }

        public List<CuentaModel> RecuperarCuentasEnLaSala()
        {
            List<CuentaModel> CuentasEnSala = new List<CuentaModel>();
            foreach(CuentaModel cuenta in CuentasEnLaSala.Keys)
            {
                CuentasEnSala.Add(cuenta);
            }
            return CuentasEnSala;
        }
    }
}
