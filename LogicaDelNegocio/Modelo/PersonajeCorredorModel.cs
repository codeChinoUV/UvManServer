using System;
using System.Runtime.Serialization;

namespace LogicaDelNegocio.Modelo
{
    /// <summary>
    /// Contiene la información de un corredor de la partida
    /// </summary>
    [DataContract]
    public class PersonajeCorredorModel
    {
        [DataMember]
        public double Velocidad { get; set; }
        [DataMember]
        public String Nombre { get; set; }
        [DataMember]
        public String Poder { get; set; }
        [DataMember]
        public int Precio { get; set; }

    }
}
