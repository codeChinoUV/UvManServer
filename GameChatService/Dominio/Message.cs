using System;
using System.Runtime.Serialization;
using LogicaDelNegocio.Modelo;

namespace GameChatService.Dominio
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public CuentaModel Remitente { get; set; }
        [DataMember]
        public String Mensaje { get; set; }
        [DataMember]
        public DateTime HoraEnvio { get; set; }
    }
}
