using System;
using System.Runtime.Serialization;

namespace LogicaDelNegocio.Modelo
{
    /// <summary>
    /// Clase Cuenta
    /// Contiene todos los atributos para la autenticación del usuario
    /// </summary>
    
    [DataContract]
    public class CuentaModel
    {
        [DataMember]
        public String nombreUsuario { get; set; }
        [DataMember]
        public String contrasena { get; set; }
        [DataMember]
        public UsuarioModel informacionDeUsuario { get; set; }
        [DataMember]
        public Boolean verificado { get; set; }
        [DataMember]
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
