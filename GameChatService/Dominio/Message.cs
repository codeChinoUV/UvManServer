using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LogicaDelNegocio.Modelo;

namespace GameChatService.Dominio
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public CuentaModel remitente { get; set; }
        [DataMember]
        public String mensaje { get; set; }
        [DataMember]
        public DateTime horaEnvio { get; set; }
    }
}
