using System.Runtime.Serialization;

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
