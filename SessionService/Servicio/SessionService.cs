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

namespace SessionService.Servicio
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SessionService : ISessionService
    {
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
                    if (manejadorDeSesiones.AgregarCuentaLogeada(cuentaCompleta))
                    {
                        return EnumEstadoInicioSesion.InicioSesionCorrecto;
                    }
                    else
                    {
                        return EnumEstadoInicioSesion.SeEncuentraLogeada;
                    }
                }
                return (EnumEstadoInicioSesion) existeCuenta ;
            }catch(EntityException ex)
            {
                //throw;
                return EnumEstadoInicioSesion.ErrorBaseDatos;
            }
        }
    }
}
