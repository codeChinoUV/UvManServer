using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LogicaDelNegocio.Modelo
{
    /// <summary>
    /// Clase Avance
    /// Contiene todo el progreso del jugador
    /// </summary>
    [DataContract]
    public class AvanceModel
    {
        [DataMember]
        public int uvCoins { get; set; }
        [DataMember]
        public int mejorPuntuacion { get; set; }
        [DataMember]
        public List<PersonajeCorredorModel> corredoresAdquiridos { get; set; }

    }
}
