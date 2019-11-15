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
        public int UvCoins { get; set; }
        [DataMember]
        public int MejorPuntuacion { get; set; }
        [DataMember]
        public List<PersonajeCorredorModel> CorredoresAdquiridos { get; set; }

    }
}
