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
        public String NombreUsuario { get; set; }
        [DataMember]
        public String Contrasena { get; set; }
        [DataMember]
        public UsuarioModel InformacionDeUsuario { get; set; }
        [DataMember]
        public Boolean Verificado { get; set; }
        [DataMember]
        public String CodigoVerificacion { get; set; }
        
        /// <summary>
        /// Verifica si el codigo de verificacion mandado como parametro coincide con el de la cuenta
        /// </summary>
        /// <param name="Codigo">String</param>
        /// <returns>Boolean</returns>
        public Boolean VerificarCuenta(String Codigo)
        {
            return Codigo == CodigoVerificacion;
        }
    }
}
