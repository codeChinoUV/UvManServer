using GameService.Contrato;
using GameService.Dominio.Enum;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.Util;
using System;
using System.Collections.Generic;


namespace GameService.Dominio
{
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
        /// <param name="salaASuscribirse"></param>
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
        public EnumEstadoDeUnirseASala UnirseASalaConId(String Id, CuentaModel CuentaAAgregar, IGameServiceCallback ActualCallback )
        {
            EnumEstadoDeUnirseASala estadoDeUnirseASala = EnumEstadoDeUnirseASala.NoSeEncuentraEnSesion;
            if (ManejadorDeSesiones.VerificarCuentaLogeada(CuentaAAgregar))
            {
                foreach (Sala sala in SalasCreadas)
                {
                    if (sala.Id == Id)
                    {
                        if (sala.UnirseASala(CuentaAAgregar, ActualCallback))
                        {
                            estadoDeUnirseASala = EnumEstadoDeUnirseASala.UnidoCorrectamente;
                            SeUnioASala?.Invoke(sala.Id);
                        }
                        else
                        {
                            estadoDeUnirseASala = EnumEstadoDeUnirseASala.SalaLlena;
                        }
                    }
                }
            }
            estadoDeUnirseASala = EnumEstadoDeUnirseASala.SalaInexistente;
            return estadoDeUnirseASala;
        }

        /// <summary>
        /// Agrega una Cuenta a una sala completa, si no hay salas disponibles la crea
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <param name="ActualCallback">IGameServiceCallback</param>
        /// <returns>Boolean</returns>
        public Boolean UnisrseASalaDisponible(CuentaModel Cuenta, IGameServiceCallback ActualCallback)
        {
            Boolean seUnioASala = false;
            if (ManejadorDeSesiones.VerificarCuentaLogeada(Cuenta))
            {
                Sala SalaAUnirse = BuscarSalaIncompleta();
                if (SalaAUnirse != null)
                {
                    SalaAUnirse.UnirseASala(Cuenta, ActualCallback);
                    seUnioASala = true;
                    SeUnioASala?.Invoke(SalaAUnirse.Id);
                }
                else
                {
                    Sala NuevaSala = CrearSalaConIdAleatorio(Cuenta, ActualCallback);
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
            IGameServiceCallback ActualCallback)
        {
            EnumEstadoCrearSalaConId EstadoDeCreacionDeSala = EnumEstadoCrearSalaConId.NoSeEncuentraEnSesion;
            if (!EstaElIdDeSalaEnUso(Id))
            {
                Sala SalaAAgregar = new Sala(Id, EsSalaPublica, Cuenta, ActualCallback);
                SalasCreadas.Add(SalaAAgregar);
                SuscribirseAEventosDeSala(SalaAAgregar);
                SalaCreada?.Invoke(SalaAAgregar);
                EstadoDeCreacionDeSala = EnumEstadoCrearSalaConId.CreadaCorrectamente;
            }
            else
            {
                EstadoDeCreacionDeSala = EnumEstadoCrearSalaConId.IdYaExistente;
            }
            return EstadoDeCreacionDeSala;
        }

        /// <summary>
        /// Verifica si la cuenta se encuentra en una sala
        /// </summary>
        /// <param name="Cuenta"></param>
        /// <returns></returns>
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
        /// <returns>List</returns>
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
        /// <returns>Sala</returns>
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
        /// <returns>Sala</returns>
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
        /// <returns>Boolean</returns>
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
        /// <returns>Sala</returns>
        private Sala CrearSalaConIdAleatorio(CuentaModel Cuenta, IGameServiceCallback ActualCallback)
        {
            String IdDeSala;
            do
            {
                IdDeSala = GeneradorCodigo.GenerarCadena();
            } while (EstaElIdDeSalaEnUso(IdDeSala));
            Sala NuevaSala = new Sala(IdDeSala, true, Cuenta, ActualCallback);
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
    }
}
