using System.Collections.Generic;
using System.Runtime.Serialization;
using AccesoDatos;
using LogicaDelNegocio.Modelo.Enum;

namespace LogicaDelNegocio.Modelo
{
    /// <summary>
    /// Contiene toda la información del usuario que se almacenara en la base de datos
    /// </summary>
    [DataContract]
    public class JugadorModel
    {
        [DataMember]
        public int MejorPuntacion  { get; set; }
        [DataMember]
        public int UvCoins { get; set; }
        [DataMember]
        public List<CorredorAdquiridoModel> CorredoresAdquiridos { get; set; }
        [DataMember]
        public List<SeguidorAdquiridoModel> SeguidoresAdquiridos { get; set; }
        [DataMember]
        public EnumTipoDeJugador RolDelJugador { get; set; }
        [DataMember] 
        public CorredorAdquiridoModel CorredorSeleccionado { get; set; }
        [DataMember]
        public PerseguidorAdquirido PerseguidorSeleccionado { get; set; }
    }
}
