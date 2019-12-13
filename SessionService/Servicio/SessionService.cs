using System;
using System.ServiceModel;
using SessionService.Contrato;
using SessionService.Dominio.Enum;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.DataAccess;
using LogicaDelNegocio.DataAccess.Interfaces;
using System.Data.Entity.Core;
using System.Diagnostics;
using LogicaDelNegocio.Util;
using System.Threading;

namespace SessionService.Servicio
{
    /// <summary>
    /// Se encarga de ofrecer los servicios de sesion
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple,
    UseSynchronizationContext = false)]

    public class SessionService : ISessionService
    {
        public ISessionServiceCallback ActualCallback {
            get {
                return OperationContext.Current.GetCallbackChannel<ISessionServiceCallback>();
            }
        }

        /// <summary>
        /// Termina la sesion de una ceunta en el servidor
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        public void CerrarSesion(CuentaModel Cuenta)
        {
            SessionManager ManejadorDeSesiones = SessionManager.GetSessionManager();
            ManejadorDeSesiones.QuitarCuentaLogeada(Cuenta);
        }

        /// <summary>
        /// Inicia sesion en el servidor si las credenciales pasadas en cuenta son validas
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
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
                    if (ManejadorDeSesiones.AgregarCuentaLogeada(CuentaCompleta, HiloDeSeguimientoDeCliente))
                    {
                        return EnumEstadoInicioSesion.InicioSesionCorrecto;
                    }

                    return EnumEstadoInicioSesion.SeEncuentraLogeada;
                }
                return (EnumEstadoInicioSesion) ExisteCuenta ;
            }catch(EntityException exception)
            {
                Debug.Write(exception.Message);
                return EnumEstadoInicioSesion.ErrorBaseDatos;
            }
        }

        /// <summary>
        /// Crea un hilo para seguir el estado del cliente
        /// </summary>
        /// <param name="CuentaASeguir">CuentaModel</param>
        /// <param name="CallbackActual">ISessionService</param>
        /// <returns>Un hilo que esta siguiendo la conexion con el cliente</returns>
        private Thread SeguirEstadoDelCliente(CuentaModel CuentaASeguir, ISessionServiceCallback CallbackActual)
        {
            EstadoCliente EstadoCliente = new EstadoCliente(CallbackActual,CuentaASeguir);
            Thread HiloEstadoDelCliente = new Thread(EstadoCliente.ChecarEstadoDelCliente);
            HiloEstadoDelCliente.Start();
            return HiloEstadoDelCliente;
        }

    }

    class EstadoCliente
    {
        
        private ISessionServiceCallback ActualCallback;
        private CuentaModel CuentaSiguiendo;
        private const int TIEMPO_ESPERA_CHECAR_CLIENTE = 10000;

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
                catch (TimeoutException)
                {
                    ManejadorDeSesiones.QuitarCuentaLogeada(CuentaSiguiendo);
                }
            }
        }
    }
}
