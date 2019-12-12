using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.Util;
using GameChatService.Servicio;
using System;
using System.Collections.Generic;
using GameChatService.Contrato;
using System.ServiceModel;
using GameService.Dominio;
using System.Diagnostics;
using Message = GameChatService.Dominio.Message;

namespace GameChatService.Servicio.TestsChat
{
    [TestClass()]
    public class ChatServiceTests
    {
        [TestMethod()]
        public void ConectarTest()
        {
            SessionManager ManejadorDeSesiones = SessionManager.GetSessionManager();
            if (ActualCallback != null)
            {
                ManejadorDeSesiones.UsuarioDesconectado += CerroSesionGlobal;
                lock (SincronizarObjeto)
                {
                    if (!CuentasConetadas.ContainsValue(ActualCallback) && !ExisteCuenta(Cuenta.NombreUsuario) &&
                        ManejadorDeSesiones.VerificarCuentaLogeada(Cuenta))
                    {
                        lock (SincronizarObjeto)
                        {
                            NotificarClientesNuevoConectado(Cuenta);
                            CuentasConetadas.Add(Cuenta, ActualCallback);
                        }
                        Cuentas.Add(Cuenta);
                        return true;
                    }
                }
            }
            return false;
            Assert.Fail();
        }
    }
}