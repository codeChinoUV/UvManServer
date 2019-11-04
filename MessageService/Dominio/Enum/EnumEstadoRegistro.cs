using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Dominio.Enum
{
    [DataContract]
    public enum EnumEstadoRegistro
    {
        [EnumMember]
        RegistroCorrecto = 1,
        [EnumMember]
        UsuarioExistente = 0,
        [EnumMember]
        ErrorEnBaseDatos = -1
    }
}
