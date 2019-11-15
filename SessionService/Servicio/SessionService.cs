using System;
using System.ServiceModel;
using SessionService.Contrato;
using SessionService.Dominio.Enum;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.DataAccess;
using LogicaDelNegocio.DataAccess.Interfaces;
using System.Data.Entity.Core;
using LogicaDelNegocio.Util;
using System.Threading;

namespace SessionService.Servicio
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple,
    UseSynchronizationContext = false)]

    public class SessionService : ISessionService
    {

        private object ObjetoDeSincronizacion = new object();

        public ISessionServiceCallback ActualCallback {
            get {
                return OperationContext.Current.GetCallbackChannel<ISessionServiceCallback>();
            }
        }

        /// <summary>
        /// Termina la sesion de una ceunta en el servidor
        /// </summary>
        /// <param name="Cuenta"></param>
        public void CerrarSesion(CuentaModel Cuenta)
        {
            SessionManager ManejadorDeSesiones = SessionManager.GetSessionManager();
            ManejadorDeSesiones.QuitarCuentaLogeada(Cuenta);
        }

        /// <summary>
        /// Inicia sesion en el servidor si las credenciales pasadas en cuenta son validas
        /// </summary>
        /// <param name="Cuenta"></param>
        /// <returns>EnumEstadoInicioSesion</returns>
        public EnumEstadoInicioSesion IniciarSesion(CuentaModel Cuenta)
        {
            ICuentaDAO PersistenciaCuenta = new CuentaDAO();
            try
            {
                int ExisteCuenta = PersistenciaCuenta.IniciarSesion(Cuenta);
                if (ExisteCuenta == 1)
                {
                    CuentaModel CuentaCompleta = PersistenciaCuenta.RecuperarCuenta(Cuenta);
                    SessionManager ManejadorDeSesiones = SessionManager.GetSessionManager();
                    Thread HiloDeSeguimientoDeCliente = SeguirEstadoDelCliente(CuentaCompleta,ActualCallback);
                    if (ManejadorDeSesiones.AgregarCuentaLogeada(CuentaCompleta, null))
                    {
                        return EnumEstadoInicioSesion.InicioSesionCorrecto;
                    }
                    else
                    {
                        return EnumEstadoInicioSesion.SeEncuentraLogeada;
                    }
                }
                return (EnumEstadoInicioSesion) ExisteCuenta ;
            }catch(EntityException)
            {
                return EnumEstadoInicioSesion.ErrorBaseDatos;
            }
        }

        /// <summary>
        /// Crea un hilo para seguir el estado del cliente
        /// </summary>
        /// <param name="CuentaASeguir">CuentaModel</param>
        /// <param name="CallbackActual">ISessionService</param>
        /// <returns>Thread</returns>
        private Thread SeguirEstadoDelCliente(CuentaModel CuentaASeguir, ISessionServiceCallback CallbackActual)
        {
            EstadoCliente EstadoCliente = new EstadoCliente(ActualCallback,CuentaASeguir);
            Thread HiloEstadoDelCliente = new Thread(new ThreadStart(EstadoCliente.ChecarEstadoDelCliente));
            HiloEstadoDelCliente.Start();
            return HiloEstadoDelCliente;
        }

    }

    class EstadoCliente
    {
        
        private ISessionServiceCallback ActualCallback;
        private CuentaModel CuentaSiguiendo;
        private const int TIEMPO_ESPERA_CHECAR_CLIENTE = 2500;

        public EstadoCliente(ISessionServiceCallback ActualCallback, CuentaModel CuentaSiguiendo)
        {
            this.ActualCallback = ActualCallback;
            this.CuentaSiguiendo = CuentaSiguiendo;
        }

        /// <summary>
        /// Monitorea el estado de un cliente llamado
        /// </summary>
        public void ChecarEstadoDelCliente()
        {
            SessionManager ManejadorDeSesiones = SessionManager.GetSessionManager();
            Thread.Sleep(TIEMPO_ESPERA_CHECAR_CLIENTE);
            if (ActualCallback != null)
            {
                try
                {
                    Boolean EstaVivo = false;
                    do
                    {
                        EstaVivo = ActualCallback.EstaVivo();
                        Thread.Sleep(TIEMPO_ESPERA_CHECAR_CLIENTE);
                    } while (EstaVivo);
                }
                catch (ObjectDisposedException)
                {
                    ManejadorDeSesiones.QuitarCuentaLogeada(CuentaSiguiendo);
                }
                catch (CommunicationException)
                {
                    ManejadorDeSesiones.QuitarCuentaLogeada(CuentaSiguiendo);
                }
            }
        }
    }
}
