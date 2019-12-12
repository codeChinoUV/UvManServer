using Microsoft.VisualStudio.TestTools.UnitTesting;
using CuentaService.Servicio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDelNegocio.Modelo;
using MessageService.Dominio.Enum;

namespace CuentaService.Servicio.Tests
{
    [TestClass()]
    public class CuentaServiceTests
    {
        [TestMethod()]
        public void RegistrarseTest()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "",
                Contrasena = "",
                Jugador = null,
                Verificado = false,
                CodigoVerificacion = "",
                CorreoElectronico = ""
            };
            EnumEstadoRegistro expectedResut = EnumEstadoRegistro.ErrorEnBaseDatos;

            CuentaService service = new CuentaService();
            
            Assert.AreEqual(expectedResut,service.Registrarse(null));

        }

        [TestMethod()]
        public void RegistrarseTestRegistroExitoso()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };
            EnumEstadoRegistro expectedResut = EnumEstadoRegistro.RegistroCorrecto;

            CuentaService service = new CuentaService();

            Assert.AreEqual(expectedResut, service.Registrarse(cuenta));

        }

        [TestMethod()]
        public void VerificarCuentaTest()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };
            EnumEstadoVerificarCuenta expectedResut = EnumEstadoVerificarCuenta.NoCoincideElCodigo;

            CuentaService service = new CuentaService();

            Assert.AreEqual(expectedResut, service.VerificarCuenta("432",cuenta));
        }

        public void VerificarCuentaTestErrorr()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = " "
            };
            EnumEstadoVerificarCuenta expectedResut = EnumEstadoVerificarCuenta.ErrorEnBaseDatos;

            CuentaService service = new CuentaService();

            Assert.AreEqual(expectedResut, service.VerificarCuenta(" ", cuenta));
        }

    }
}