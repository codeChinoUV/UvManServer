using LogicaDelNegocio.Modelo;
using System;
using System.Collections.Generic;
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
        /// <param name="cuenta">CuentaModel</param>
        /// <param name="hiloDeSeguimientoDelCliente">Thread</param>
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
                CuentasLogeadas.Remove(cuentaActual);
                UsuarioDesconectado?.Invoke(cuentaActual);
                hiloDeSeguimientoDelCliente?.Abort();
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
        
        /// <summary>
        /// Regresa la cuenta con todos los datos que tiene almacenada la sesion
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        /// <returns>CuentaModel</returns>
        public CuentaModel ObtenerCuentaCompleta(CuentaModel cuenta)
        {
            CuentaModel CuentaCompleta = null;
            foreach (CuentaModel cuentaLoegada in CuentasLogeadas.Keys)
            {
                if (cuenta.NombreUsuario == cuentaLoegada.NombreUsuario)
                {
                    CuentaCompleta = cuentaLoegada;
                    break;
                }
            }
            return CuentaCompleta;
        }
    }
}
