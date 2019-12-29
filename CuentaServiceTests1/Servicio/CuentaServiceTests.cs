using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicaDelNegocio.Modelo;
using MessageService.Dominio.Enum;

namespace CuentaService.Servicio.Tests
{
    [TestClass()]
    public class CuentaServiceTests
    {
        [TestMethod()]
        public void RegistrarseTestUsuarioExistente()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "chino",
                Contrasena = "joseMiguel",
                Jugador = null,
                Verificado = false,
                CodigoVerificacion = "",
                CorreoElectronico = "pumas.chino99@gmail.com"
            };
            EnumEstadoRegistro expectedResut = EnumEstadoRegistro.UsuarioExistente;

            CuentaService service = new CuentaService();

            Assert.AreEqual(expectedResut,service.Registrarse(null));

        }

        [TestMethod()]
        public void RegistrarseTestRegistroExitoso()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "chino3",
                Contrasena = "hola9011",
                CorreoElectronico = "pumas.chino99@gmail.com"
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

    }
}