using GameService.Contrato;
using GameService.Dominio.Enum;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Dominio
{
    public sealed class SalaManager
    {
        private static SalaManager ManejadorDeSala = new SalaManager();
        private static List<Sala> SalasCreadas = new List<Sala>();
        private SessionManager ManejadorDeSesiones = SessionManager.GetSessionManager();

        public static SalaManager GetSalaManager()
        {
            return ManejadorDeSala;
        }

        public void EliminarSala(Sala SalaAEliminar)
        {
            SalasCreadas.Remove(SalaAEliminar);
        }

        public EnumEstadoDeUnirseASala UnirseASalaConId(String Id, CuentaModel CuentaAAgregar, IGameServiceCallback ActualCallback )
        {
            EnumEstadoDeUnirseASala estadoDeUnirseASala = EnumEstadoDeUnirseASala.NoSeEncuentraEnSesion;
            if (ManejadorDeSesiones.VerificarCuentaLogeada(CuentaAAgregar))
            {
                foreach (Sala sala in SalasCreadas)
                {
                    if (sala.Id == Id)
                    {
                        if (sala.UnirseASala(CuentaAAgregar, ActualCallback))
                        {
                            estadoDeUnirseASala = EnumEstadoDeUnirseASala.UnidoCorrectamente;
                        }
                        else
                        {
                            estadoDeUnirseASala = EnumEstadoDeUnirseASala.SalaLlena;
                        }
                    }
                }
            }
            estadoDeUnirseASala = EnumEstadoDeUnirseASala.SalaInexistente;
            return estadoDeUnirseASala;
        }

        public Boolean UnisrseASalaDisponible(CuentaModel Cuenta, IGameServiceCallback ActualCallback)
        {
            Boolean SeUnioASala = false;
            if (ManejadorDeSesiones.VerificarCuentaLogeada(Cuenta))
            {
                Sala SalaAUnirse = BuscarSalaIncompleta();
                if (SalaAUnirse != null)
                {
                    SalaAUnirse.UnirseASala(Cuenta, ActualCallback);
                    SeUnioASala = true;
                }
                else
                {
                    SalasCreadas.Add(CrearSalaConIdAleatorio(Cuenta, ActualCallback));
                    SeUnioASala = true;
                }
            }
            return SeUnioASala;
        }

        public EnumEstadoCrearSalaConId CrearSala(string Id, Boolean EsSalaPublica, CuentaModel Cuenta, 
            IGameServiceCallback ActualCallback)
        {
            EnumEstadoCrearSalaConId EstadoDeCreacionDeSala = EnumEstadoCrearSalaConId.NoSeEncuentraEnSesion;
            if (!IdDeSalaOcupado(Id))
            {
                Sala SalaAAgregar = new Sala(Id, EsSalaPublica, Cuenta, ActualCallback);
                SalasCreadas.Add(SalaAAgregar);
                EstadoDeCreacionDeSala = EnumEstadoCrearSalaConId.CreadaCorrectamente;
            }
            else
            {
                EstadoDeCreacionDeSala = EnumEstadoCrearSalaConId.IdYaExistente;
            }
            return EstadoDeCreacionDeSala;
        }

        public bool VerificarSiEstoyEnSala(CuentaModel Cuenta)
        {
            Boolean SeEncuentraEnSala = false;
            foreach (Sala sala in SalasCreadas)
            {
                if (sala.EstaLaCuentaEnLaSala(Cuenta))
                {
                    SeEncuentraEnSala = true;
                }
            }
            return SeEncuentraEnSala;
        }

        public List<CuentaModel> RecuperarCuentasDeSalaDeJugador(CuentaModel Cuenta)
        {
            List<CuentaModel> CuentasDeLaSala;
            Sala SalaDondeSeEncuentraLaCuenta = RecuperarSalaDeCuenta(Cuenta);
            if(SalaDondeSeEncuentraLaCuenta != null)
            {
                CuentasDeLaSala = SalaDondeSeEncuentraLaCuenta.RecuperarCuentasEnLaSala();
            }
            else
            {
                CuentasDeLaSala = new List<CuentaModel>();            
            }
            return CuentasDeLaSala;
        }

        private Sala RecuperarSalaDeCuenta(CuentaModel Cuenta)
        {
            Sala SalaDondeSeEncuentraLaCuenta = null;
            foreach(Sala sala in SalasCreadas)
            {
                if (sala.EstaLaCuentaEnLaSala(Cuenta))
                {
                    SalaDondeSeEncuentraLaCuenta = sala;
                }
            }
            return SalaDondeSeEncuentraLaCuenta;
        }
    
        private Sala BuscarSalaIncompleta()
        {
            Sala SalaIncompleta = null;
            foreach (Sala sala in SalasCreadas)
            {
                if (sala.EsSalaPublica && !sala.EstaLaSalaLlena())
                {
                    SalaIncompleta = sala;
                }
            }
            return SalaIncompleta;
        }

        private Boolean IdDeSalaOcupado(String idAComparar)
        {
            Boolean IdEstaOcupado = false;
            foreach (Sala sala in SalasCreadas)
            {
                if (sala.Id == idAComparar)
                {
                    IdEstaOcupado = true;
                }
            }
            return IdEstaOcupado;
        }

        private Sala CrearSalaConIdAleatorio(CuentaModel Cuenta, IGameServiceCallback ActualCallback)
        {
            String IdDeSala;
            do
            {
                IdDeSala = GeneradorCodigo.GenerarCodigoActivacion();
            } while (IdDeSalaOcupado(IdDeSala));
            Sala NuevaSala = new Sala(IdDeSala, true, Cuenta, ActualCallback);
            return NuevaSala;
        }

    }
}
