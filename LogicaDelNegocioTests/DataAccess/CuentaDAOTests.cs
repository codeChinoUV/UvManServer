using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicaDelNegocio.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDelNegocio.Modelo;

namespace LogicaDelNegocio.DataAccess.Tests
{
    [TestClass()]
    public class CuentaDAOTests
    {
        [TestMethod()]
        public void RegistrarseTest()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };
            CuentaModel expectedResut = null;
            CuentaDAO cuentaDAO = new CuentaDAO();

            Assert.AreEqual(expectedResut,cuentaDAO.Registrarse(cuenta)); 
        }
    }
}