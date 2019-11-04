using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public double velocidad { get; set; }
        [DataMember]
        public String nombre { get; set; }
        [DataMember]
        public String poder { get; set; }
        [DataMember]
        public int precio { get; set; }

        public PersonajeCorredorModel()
        {
            velocidad = 0.2;
        }
    }
}
