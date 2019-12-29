using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using LogicaDelNegocio.Modelo;
using GameService.Dominio.Enum;

namespace GameService.Servicio.Tests
{
    [TestClass()]
    public class GameServiceTests
    {

        [TestMethod()]
        public void UnirseASalaPrivadaTest()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };
            bool expectedResut = false;

            EnumEstadoDeUnirseASala estado = EnumEstadoDeUnirseASala.NoSeEncuentraEnSesion;

            GameService service = new GameService();

            Assert.AreEqual(expectedResut, service.UnirseASalaPrivada("1234", cuenta));
        }

        [TestMethod()]
        public void UnirseASalaPrivadaTestNoexisteSala()
        {
            SessionService.Servicio.SessionService ServicioSesion = new SessionService.Servicio.SessionService();
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "chino",
                Contrasena = "joseMiguel",
                CorreoElectronico = "pumas.chino99@gmail.com"
            };
            ServicioSesion.IniciarSesion(cuenta);
            bool expectedResut = true;

            string id = "qM3A20";

            GameService service = new GameService();

            Assert.AreEqual(expectedResut, service.UnirseASalaPrivada(id, cuenta));
        }

        [TestMethod()]
        public void UnirseASalaTest()
        {
            SessionService.Servicio.SessionService ServicioSesion = new SessionService.Servicio.SessionService();
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "hachi",
                Contrasena = "hola9011",
                CorreoElectronico = "pumas.chino99@gmail.com"
            };
            ServicioSesion.IniciarSesion(cuenta);
            bool expectedResut = true;

            GameService service = new GameService();

            Assert.AreEqual(expectedResut, service.UnirseASala(cuenta));
        }

        [TestMethod()]
        public void VerificarSiEstoyEnSalaTest()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };
            bool expectedResut = false;

            GameService service = new GameService();

            Assert.AreEqual(expectedResut, service.VerificarSiEstoyEnSala(cuenta));
        }

        [TestMethod()]
        public void ObtenerCuentasEnMiSalaTest()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };
            List<CuentaModel> expectedResut = new List<CuentaModel>();

            GameService service = new GameService();

            Assert.AreEqual(expectedResut, service.ObtenerCuentasEnMiSala(cuenta));
        }

        [TestMethod()]
        public void RecuperarIdDeMiSalaTest()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };
            string expectedResut = String.Empty;

            GameService service = new GameService();

            Assert.AreEqual(expectedResut, service.RecuperarIdDeMiSala(cuenta));
        }

        [TestMethod()]
        public void MiSalaEsPublicaTest()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };

            bool expectedResut = false;

            GameService service = new GameService();

            Assert.AreEqual(expectedResut, service.MiSalaEsPublica(cuenta));
        }
        public void MiSalaEsPublicaTestExito()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };

            bool expectedResut = true;

            GameService service = new GameService();

            Assert.AreEqual(expectedResut, service.MiSalaEsPublica(cuenta));
        }


        [TestMethod()]
        public void RecuperarMejoresPuntuacionesTest()
        {
            List<CuentaModel> ListaDeCuentas = new List<CuentaModel>();
            ListaDeCuentas.Add(new CuentaModel());
            ListaDeCuentas.Add(new CuentaModel());
            ListaDeCuentas.Add(new CuentaModel());
            ListaDeCuentas.Add(new CuentaModel());
            ListaDeCuentas.Add(new CuentaModel());
            int expectedResult = ListaDeCuentas.Count;
            GameService service = new GameService();

            Assert.AreEqual(expectedResult, service.RecuperarMejoresPuntuaciones().Count);
        }

    }
}