using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameChatService.Dominio
{
    [DataContract]
    public class Cuenta
    {
        [DataMember]
        public String usuario { get; set; }

        [DataMember]
        public String contrasena { get; set; }

        [DataMember]
        public Usuario datosDelUsuario { get; set; }

        [DataMember]
        public Boolean verificada { get; set; }

        [DataMember]
        public String codigoVerificacion { get; set; }
    }
}
