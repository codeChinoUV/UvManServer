using System;
using System.Runtime.Serialization;

namespace LogicaDelNegocio.Modelo
{
    /// <summary>
    /// Clase Usuario
    /// Contiene toda la información del usuario que se almacenara en la base de datos
    /// </summary>
    [DataContract]
    public class UsuarioModel
    {
        [DataMember]
        public String Correo { get; set; }
        [DataMember]
        public String Edad { get; set; }
        [DataMember]
        public AvanceModel AvanceDeUsuario { get; set; }

    }
}
