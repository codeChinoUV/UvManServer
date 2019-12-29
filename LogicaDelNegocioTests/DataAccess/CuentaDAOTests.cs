using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                NombreUsuario = "chino4",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };
            CuentaModel expectedResut = new CuentaModel()
            {
                NombreUsuario = "chino4"
            };
            CuentaDAO cuentaDAO = new CuentaDAO();

            Assert.AreEqual(expectedResut.NombreUsuario,cuentaDAO.Registrarse(cuenta).NombreUsuario); 
        }
    }
}