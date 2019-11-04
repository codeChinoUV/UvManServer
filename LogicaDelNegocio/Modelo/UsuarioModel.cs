using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public String correo { get; set; }
        [DataMember]
        public String edad { get; set; }
        [DataMember]
        public AvanceModel avanceDeUsuario { get; set; }

    }
}
