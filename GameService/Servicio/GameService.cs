using System;
using System.Collections.Generic;
using GameService.Contrato;
using GameService.Dominio;
using GameService.Dominio.Enum;
using LogicaDelNegocio.Modelo;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace GameService.Servicio
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple,
    UseSynchronizationContext = false)]
    public class GameService : IGameService
    {
        private object ObjetoDeSincroinzacion = new object();
        private SalaManager ManejadorDeSala = SalaManager.GetSalaManager();

        public IGameServiceCallback ActualCallback {
            get {
                return OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
            }
        }

        public String DireccionIpDelCliente
        {
            get
            {
                OperationContext ContextoActual = OperationContext.Current;
                MessageProperties PropiedadesDelContexto = ContextoActual.IncomingMessageProperties;
                RemoteEndpointMessageProperty PropiedadesDelPuntoDeLlegada = (RemoteEndpointMessageProperty)
                    PropiedadesDelContexto[RemoteEndpointMessageProperty.Name];
                return PropiedadesDelPuntoDeLlegada.Address;
            }
        }

        /// <summary>
        /// Crea una sala personalizada
        /// </summary>
        /// <param name="Id">String</param>
        /// <param name="EsSalaPublica">Boolean</param>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>EnumEstadoCrearSalaConId</returns>
        public EnumEstadoCrearSalaConId CrearSala(string Id, bool EsSalaPublica, CuentaModel Cuenta)
        {
            return ManejadorDeSala.CrearSala(Id, EsSalaPublica, Cuenta, ActualCallback, DireccionIpDelCliente);
        }

        /// <summary>
        /// Se une una cuenta a una sala privada
        /// </summary>
        /// <param name="Id">Stirng</param>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>EnumEstadoDeUnirseASala</returns>
        public EnumEstadoDeUnirseASala UnirseASalaPrivada(string Id, CuentaModel Cuenta)
        {
            return ManejadorDeSala.UnirseASalaConId(Id, Cuenta, ActualCallback, DireccionIpDelCliente);
        }

        /// <summary>
        /// Se une una cuenta a una sala disponible
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>Boolean</returns>
        public bool UnirseASala(CuentaModel Cuenta)
        {
            return ManejadorDeSala.UnisrseASalaDisponible(Cuenta, ActualCallback, DireccionIpDelCliente);
        }

        /// <summary>
        /// Verifica si un usuario se encuentra en una sala
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>Boolean</returns>
        public bool VerificarSiEstoyEnSala(CuentaModel Cuenta)
        {
            return ManejadorDeSala.VerificarSiEstoyEnSala(Cuenta);
        }

        /// <summary>
        /// Regresa las cuentas en las que se encuentra en la sala de la Cuenta
        /// </summary>
        /// <param name="Cuenta"></param>
        /// <returns>List</returns>
        public List<CuentaModel> ObtenerCuentasEnMiSala(CuentaModel Cuenta)
        {
            return ManejadorDeSala.RecuperarCuentasDeSalaDeJugador(Cuenta);
        }

        /// <summary>
        /// Regresa el Id de la sala en donde se encuentra la Cuenta
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>String</returns>
        public string RecuperarIdDeMiSala(CuentaModel Cuenta)
        {
            Sala MiSala = ManejadorDeSala.RecuperarSalaDeCuenta(Cuenta);
            if(MiSala != null)
            {
                return MiSala.Id;
            }
            return string.Empty;
        }

        /// <summary>
        /// Regresa si la sala de la Cuenta es publica
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>Boolean</returns>
        public bool MiSalaEsPublica(CuentaModel Cuenta)
        {
            Sala MiSala = ManejadorDeSala.RecuperarSalaDeCuenta(Cuenta);
            if (MiSala != null)
            {
                return MiSala.EsSalaPublica;
            }
            return false;
        }
    }
}
