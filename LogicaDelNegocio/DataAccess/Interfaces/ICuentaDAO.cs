using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDelNegocio.Modelo;


namespace LogicaDelNegocio.DataAccess.Interfaces
{
    public interface ICuentaDAO
    {
        CuentaModel CheckIn(CuentaModel cuentaNueva);

        int LogIn(CuentaModel cuenta);

        Boolean VerifyAccount(CuentaModel cuenta);

        CuentaModel RecuperarCuenta(CuentaModel cuentaARecuperar);
        //List<CuentaModel> getBestScores();
    }
}
