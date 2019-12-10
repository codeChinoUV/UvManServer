using LogicaDelNegocio.Modelo;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LogicaDelNegocio.Util
{
    /// <summary>
    /// Se encarga de manejar a las cuentas que se encuentran en sesion
    /// </summary>
    public sealed class SessionManager
    {
        private static Dictionary<CuentaModel, Thread> CuentasLogeadas = new Dictionary<CuentaModel, Thread>();
        private static SessionManager ManejadorDeSesiones = new SessionManager();
        public delegate void NotificacionSobreUsuario(CuentaModel cuenta);
        public event NotificacionSobreUsuario UsuarioDesconectado;
        public event NotificacionSobreUsuario UsuarioConectado;
        private Object ObjetoSincronizador = new object();

        private SessionManager()
        {
        }


        /// <summary>
        /// Retorna una instancia singleton del manejador de sesiones
        /// </summary>
        /// <returns>La instancia del SessionManager</returns>
        public static SessionManager GetSessionManager()
        {
            return ManejadorDeSesiones;
        }

        /// <summary>
        /// Agrega una CuentaModelYaLogeada en la sesion, si esta aun no se encuentra en la sesion
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        /// <param name="hiloDeSeguimientoDelCliente">Thread</param>
        /// <returns>Verdadeo si la cuenta se logeo correctamente, falso si no</returns>
        public Boolean AgregarCuentaLogeada(CuentaModel cuenta, Thread hiloDeSeguimientoDelCliente)
        {
            lock (ObjetoSincronizador)
            {
                foreach (CuentaModel cuentaLogeada in CuentasLogeadas.Keys)
                {
                    if (cuentaLogeada.NombreUsuario == cuenta.NombreUsuario)
                    {
                        return false;
                    }
                }
                CuentasLogeadas.Add(cuenta, hiloDeSeguimientoDelCliente);
            }
            UsuarioConectado?.Invoke(cuenta);
            return true;
        }
        

        /// <summary>
        /// Elimina la cuenta de la sesion
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        public void QuitarCuentaLogeada(CuentaModel cuenta)
        {
            CuentaModel cuentaActual = null;
            lock (ObjetoSincronizador)
            {
                foreach (CuentaModel cuentaEnElDiccionario in CuentasLogeadas.Keys)
                {
                    if (cuentaEnElDiccionario.NombreUsuario == cuenta.NombreUsuario)
                    {
                        cuentaActual = cuentaEnElDiccionario;
                    }
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
        /// <returns>Verdadero si la cuenta se cuentra en la sesion, falso si no</returns>
        public Boolean VerificarCuentaLogeada(CuentaModel cuenta)
        {
            lock (ObjetoSincronizador)
            {
                foreach (CuentaModel cuentaLogeada in CuentasLogeadas.Keys)
                {
                    if (cuentaLogeada.NombreUsuario == cuenta.NombreUsuario)
                    {
                        return true;
                    }
                }
            }
            return false;
        }    
        
        /// <summary>
        /// Regresa la cuenta con todos los datos que tiene almacenada la sesion
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        /// <returns>La cuentaModel que contiene los datos completos de la cuenta que se le pasa como parametro</returns>
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

        /// <summary>
        /// Termina a todos los hilos que estaban supervisando la conexion de los clientes.
        /// </summary>
        public void TerminarTodosLosHilosDeEscucha()
        {
            foreach (Thread hilo in CuentasLogeadas.Values)
            {
                hilo?.Abort();
            }
        }
    }
}
