using LogicaDelNegocio.Modelo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LogicaDelNegocio.Util
{
    public sealed class SessionManager
    {
        private static Dictionary<CuentaModel, Thread> CuentasLogeadas = new Dictionary<CuentaModel, Thread>();
        private static SessionManager ManejadorDeSesiones = new SessionManager();
        public delegate void NotificacionSobreUsuario(CuentaModel cuenta);
        public event NotificacionSobreUsuario UsuarioDesconectado;
        public event NotificacionSobreUsuario UsuarioConectado;

        private SessionManager()
        {
        }


        /// <summary>
        /// Retorna una instancia singleton del manejador de sesiones
        /// </summary>
        /// <returns>SessionManager</returns>
        public static SessionManager GetSessionManager()
        {
            return ManejadorDeSesiones;
        }

        /// <summary>
        /// Agrega una CuentaModelYaLogeada en la sesion, si esta aun no se encuentra en la sesion
        /// </summary>
        /// <param name="cuenta"></param>
        /// <returns>Boolean</returns>
        public Boolean AgregarCuentaLogeada(CuentaModel cuenta, Thread hiloDeSeguimientoDelCliente)
        {
            foreach(CuentaModel cuentaLogeada in CuentasLogeadas.Keys)
            {
                if(cuentaLogeada.NombreUsuario == cuenta.NombreUsuario)
                {
                    return false;
                }
            }
            UsuarioConectado?.Invoke(cuenta);
            CuentasLogeadas.Add(cuenta, hiloDeSeguimientoDelCliente);
            return true;
        }
        

        /// <summary>
        /// Elimina la cuenta de la sesion
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        public void QuitarCuentaLogeada(CuentaModel cuenta)
        {
            CuentaModel cuentaActual = null;
            foreach(CuentaModel cuentaEnElDiccionario in CuentasLogeadas.Keys)
            {
                if(cuentaEnElDiccionario.NombreUsuario == cuenta.NombreUsuario)
                {
                    cuentaActual = cuentaEnElDiccionario;
                }
            }
            if(cuentaActual != null)
            {
                Thread hiloDeSeguimientoDelCliente = CuentasLogeadas[cuentaActual];
                if (hiloDeSeguimientoDelCliente != null)
                {
                    hiloDeSeguimientoDelCliente.Abort();
                }
                CuentasLogeadas.Remove(cuentaActual);
                UsuarioDesconectado?.Invoke(cuentaActual);
            }
        }


        /// <summary>
        /// Verifica si una cuenta ya se encuentra en la sesion
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        /// <returns>Boolean</returns>
        public Boolean VerificarCuentaLogeada(CuentaModel cuenta)
        {
            foreach(CuentaModel cuentaLogeada in CuentasLogeadas.Keys)
            {
                if(cuentaLogeada.NombreUsuario == cuenta.NombreUsuario)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
