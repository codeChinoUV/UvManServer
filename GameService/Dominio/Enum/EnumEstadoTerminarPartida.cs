using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Dominio.Enum
{
    [DataContract]
    public enum EnumEstadoTerminarPartida
    {
        [EnumMember]
        TerminadaCorrectamente = 0,
        [EnumMember]
        NoSePudoGuardarLosDatosDeLaPartida = 1,
        [EnumMember]
        SePerdioLaConexionALaBaseDeDatos = 2,
    }
}
