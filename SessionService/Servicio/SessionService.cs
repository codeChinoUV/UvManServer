using System;
using System.ServiceModel;
using SessionService.Contrato;
using SessionService.Dominio.Enum;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.DataAccess;
using LogicaDelNegocio.DataAccess.Interfaces;
using System.Data.Entity.Core;
using LogicaDelNegocio.Util;
using System.Diagnostics;
using System.Threading;

namespace SessionService.Servicio
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple,
    UseSynchronizationContext = false)]

    public class SessionService : ISessionService
    {

        private object ObjetoDeSincronizacion = new object();

        public ISessionServiceCallback actualCallback {
            get {
                return OperationContext.Current.GetCallbackChannel<ISessionServiceCallback>();
            }
        }

        /// <summary>
        /// Termina la sesion de una ceunta en el servidor
        /// </summary>
        /// <param name="cuenta"></param>
        public void CerrarSesion(CuentaModel cuenta)
        {
            SessionManager manejadorDeSesiones = SessionManager.GetSessionManager();
            manejadorDeSesiones.QuitarCuentaLogeada(cuenta);
        }

        /// <summary>
        /// Inicia sesion en el servidor si las credenciales pasadas en cuenta son validas
        /// </summary>
        /// <param name="cuenta"></param>
        /// <returns>EnumEstadoInicioSesion</returns>
        public EnumEstadoInicioSesion IniciarSesion(CuentaModel cuenta)
        {
            ICuentaDAO persistenciaCuenta = new CuentaDAO();
            Debug.WriteLine(cuenta.nombreUsuario);
            try
            {
                int existeCuenta = persistenciaCuenta.IniciarSesion(cuenta);
                if (existeCuenta == 1)
                {
                    CuentaModel cuentaCompleta = persistenciaCuenta.RecuperarCuenta(cuenta);
                    SessionManager manejadorDeSesiones = SessionManager.GetSessionManager();
                    Thread hiloDeSeguimientoDeCliente = SeguirEstadoDelCliente(cuentaCompleta,actualCallback);
                    if (manejadorDeSesiones.AgregarCuentaLogeada(cuentaCompleta, null))
                    {
                        return EnumEstadoInicioSesion.InicioSesionCorrecto;
                    }
                    else
                    {
                        return EnumEstadoInicioSesion.SeEncuentraLogeada;
                    }
                }
                return (EnumEstadoInicioSesion) existeCuenta ;
            }catch(EntityException)
            {
                return EnumEstadoInicioSesion.ErrorBaseDatos;
            }
        }

        /// <summary>
        /// Crea un hilo para seguir el estado del cliente
        /// </summary>
        /// <param name="cuentaASeguir">CuentaModel</param>
        /// <param name="callbackActual">ISessionService</param>
        /// <returns>Thread</returns>
        private Thread SeguirEstadoDelCliente(CuentaModel cuentaASeguir, ISessionServiceCallback callbackActual)
        {
            EstadoCliente estadoCliente = new EstadoCliente(actualCallback,cuentaASeguir);
            Thread hiloEstadoDelCliente = new Thread(new ThreadStart(estadoCliente.ChecarEstadoDelCliente));
            hiloEstadoDelCliente.Start();
            return hiloEstadoDelCliente;
        }

    }

    class EstadoCliente
    {
        
        private ISessionServiceCallback actualCallback;
        private CuentaModel cuentaSiguiendo;
        private int TIEMPO_ESPERA_CHECAR_CLIENTE = 2500;

        public EstadoCliente(ISessionServiceCallback actualCallback, CuentaModel cuentaSiguiendo)
        {
            this.actualCallback = actualCallback;
            this.cuentaSiguiendo = cuentaSiguiendo;
        }

        public void ChecarEstadoDelCliente()
        {
            SessionManager manejadorDeSesiones = SessionManager.GetSessionManager();
            Thread.Sleep(TIEMPO_ESPERA_CHECAR_CLIENTE);
            if (actualCallback != null)
            {
                try
                {
                    Boolean estaVivo = false;
                    do
                    {
                        estaVivo = actualCallback.EstaVivo();
                        Debug.WriteLine("Regreso true despues de un segundo " + cuentaSiguiendo.nombreUsuario);
                        Thread.Sleep(TIEMPO_ESPERA_CHECAR_CLIENTE);
                    } while (estaVivo);
                }
                catch (ObjectDisposedException)
                {
                    manejadorDeSesiones.QuitarCuentaLogeada(cuentaSiguiendo);
                }
                catch (CommunicationException)
                {
                    manejadorDeSesiones.QuitarCuentaLogeada(cuentaSiguiendo);
                }
            }
        }
    }
}
