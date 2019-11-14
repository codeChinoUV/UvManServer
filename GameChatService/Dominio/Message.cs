using System;
using System.Runtime.Serialization;
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
