using GameService.Contrato;
using GameService.Dominio;
using GameService.Dominio.Enum;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.Util;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace GameService.Servicio
{
    public class GameService : IGameService
    {
        private object ObjetoDeSincroinzacion = new object();
        private SalaManager ManejadorDeSala = SalaManager.GetSalaManager();

        public IGameServiceCallback ActualCallback {
            get {
                return OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
            }
        }

        public EnumEstadoCrearSalaConId CrearSala(string Id, bool EsSalaPublica, CuentaModel Cuenta)
        {
            return ManejadorDeSala.CrearSala(Id, EsSalaPublica, Cuenta, ActualCallback);
        }

        public EnumEstadoDeUnirseASala UnirseASala(string Id, CuentaModel Cuenta)
        {
            return ManejadorDeSala.UnirseASalaConId(Id, Cuenta, ActualCallback);
        }

        public bool UnirseASala(CuentaModel Cuenta)
        {
            return ManejadorDeSala.UnisrseASalaDisponible(Cuenta, ActualCallback);
        }

        public bool VerificarSiEstoyEnSala(CuentaModel Cuenta)
        {
            return ManejadorDeSala.VerificarSiEstoyEnSala(Cuenta);
        }

        
    }
}
