using System;
using LogicaDelNegocio.Modelo;


namespace LogicaDelNegocio.DataAccess.Interfaces
{
    public interface ICuentaDAO
    {
        CuentaModel Registrarse(CuentaModel cuentaNueva);

        int IniciarSesion(CuentaModel cuenta);

        Boolean VerificarCuenta(CuentaModel cuenta);

        CuentaModel RecuperarCuenta(CuentaModel cuentaARecuperar);

    }
}
