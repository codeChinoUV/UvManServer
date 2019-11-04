using LogicaDelNegocio.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDelNegocio.Util
{
    public sealed class SessionManager
    {
        private static List<CuentaModel> cuentasLogeadas = new List<CuentaModel>();
        private static SessionManager manejadorDeSesiones = new SessionManager();

        private SessionManager()
        {
        }

        /// <summary>
        /// Retorna una instancia singleton del manejador de sesiones
        /// </summary>
        /// <returns>SessionManager</returns>
        public static SessionManager GetSessionManager()
        {
            return manejadorDeSesiones;
        }

        /// <summary>
        /// Agrega una CuentaModelYaLogeada en la sesion, si esta aun no se encuentra en la sesion
        /// </summary>
        /// <param name="cuenta"></param>
        /// <returns>Boolean</returns>
        public Boolean AgregarCuentaLogeada(CuentaModel cuenta)
        {
            foreach(CuentaModel cuentaLogeada in cuentasLogeadas)
            {
                if(cuentaLogeada.nombreUsuario == cuenta.nombreUsuario)
                {
                    return false;
                }
            }
            cuentasLogeadas.Add(cuenta);
            return true;
        }
        
        /// <summary>
        /// Elimina la cuenta de la sesion
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        public void QuitarCuentaLogeada(CuentaModel cuenta)
        {
            cuentasLogeadas.Remove(cuenta);
        }


        /// <summary>
        /// Verifica si una cuenta ya se encuentra en la sesion
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        /// <returns>Boolean</returns>
        public Boolean VerificarCuentaLogeada(CuentaModel cuenta)
        {
            foreach(CuentaModel cuentaLogeada in cuentasLogeadas)
            {
                if(cuentaLogeada.nombreUsuario == cuenta.nombreUsuario)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
