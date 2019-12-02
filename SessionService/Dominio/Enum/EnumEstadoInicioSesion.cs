using System.Runtime.Serialization;

namespace SessionService.Dominio.Enum
{
    [DataContract]
    public enum EnumEstadoInicioSesion
    {
        [EnumMember(Value = "Correcto")]
        InicioSesionCorrecto = 1,
        [EnumMember(Value = "CredencialesInvalidas")]
        CredencialesInvalidas = 0,
        [EnumMember(Value = "CuentaNoVerificada")]
        CuentaNoVerificada = -1,
        [EnumMember(Value = "ErrorBD")]
        ErrorBaseDatos = -2,
        [EnumMember(Value = "CuentaYaLogeada")]
        SeEncuentraLogeada = -3
    }
}
