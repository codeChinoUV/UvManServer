using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameService.Servicio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDelNegocio.Modelo;

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

            string id = "1234";

            GameService service = new GameService();

            Assert.AreEqual(expectedResut, service.UnirseASalaPrivada(id, cuenta));
        }

        [TestMethod()]
        public void UnirseASalaPrivadaTestExito()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };
            bool expectedResut = true;

            string id = "qM3A20";

            GameService service = new GameService();

            Assert.AreEqual(expectedResut, service.UnirseASalaPrivada(id, cuenta));
        }

        [TestMethod()]
        public void UnirseASalaTest()
        {
            CuentaModel cuenta = new CuentaModel()
            {
                NombreUsuario = "WingXstar",
                Contrasena = "PepitoElgrillo45",
                CorreoElectronico = "man_spider.345@hotmail.com"
            };
            bool expectedResut = false;

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
            string expectedResut = null;

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
                List<CuentaModel> expectedResut = new List<CuentaModel>();

                GameService service = new GameService();

                Assert.AreEqual(expectedResut, service.RecuperarMejoresPuntuaciones());
        }

    }
}