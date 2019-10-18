using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDelNegocio.Modelo
{
    /// <summary>
    /// Clase Cuenta
    /// Contiene todos los atributos para la autenticación del usuario
    /// </summary>
    public class CuentaModel
    {
        public String nombreUsuario { get; set; }
        public String contrasena { get; set; }
        public UsuarioModel informacionDeUsuario { get; set; }
        public Boolean verificado { get; set; }
        public String codigoVerificacion { get; set; }
        
        /// <summary>
        /// Verifica si el codigo de verificacion mandado como parametro coincide con el de la cuenta
        /// </summary>
        /// <param name="codigo">String</param>
        /// <returns>Boolean</returns>
        public Boolean VerificarCuenta(String codigo)
        {
            return codigo == codigoVerificacion;
        }
    }
}
