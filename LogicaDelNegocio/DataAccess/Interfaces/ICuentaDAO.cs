﻿using System;
using System.Collections.Generic;
using LogicaDelNegocio.Modelo;


namespace LogicaDelNegocio.DataAccess.Interfaces
{
    public interface ICuentaDAO
    {
        CuentaModel Registrarse(CuentaModel CuentaNueva);
        int IniciarSesion(CuentaModel Cuenta);
        Boolean VerificarCuenta(CuentaModel Cuenta);
        CuentaModel RecuperarCuenta(CuentaModel CuentaARecuperar);
        void GuardarDatosDeLaCuenta(CuentaModel CuentaActualizar);
        List<CuentaModel> RecuperarMejoresPuntuaciones();
    }
}
