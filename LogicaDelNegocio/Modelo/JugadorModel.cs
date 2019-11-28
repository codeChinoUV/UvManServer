using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameService.Dominio.Enum;

namespace LogicaDelNegocio.Modelo
{
    /// <summary>
    /// Clase Usuario
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
        
        public EnumTipoDeJugador RolDelJugador { get; set; }
    }
}
