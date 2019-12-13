using System;
using System.Collections.Generic;
using System.Linq;
using LogicaDelNegocio.DataAccess.Interfaces;
using LogicaDelNegocio.Modelo;
using AccesoDatos;
using LogicaDelNegocio.Util;
using System.Data.Entity.Core;
using System.Diagnostics;

namespace LogicaDelNegocio.DataAccess
{
    /// <summary>
    /// Se encarga del almacenamiento de los datos de una cuenta
    /// </summary>
    public class CuentaDAO : ICuentaDAO
    {
        /// <summary>
        /// Almacena en la base de datos una cuenta si el nombre de usuario no se repite
        /// </summary>
        /// <param name="cuentaNueva">CuentaModel</param>
        /// <returns>La cuenta registrada</returns>
        public CuentaModel Registrarse(CuentaModel cuentaNueva)
        {
            CuentaModel CuentaGuardada = null;
            using (PersistenciaContainer Persistencia = new PersistenciaContainer())
            {
                int UsuariosRepetidos = ContarNombreDeUsuariosRepetidos(cuentaNueva.NombreUsuario);
                if (UsuariosRepetidos == 0)
                {
                    Cuenta CuentaAGuardar = CrearCuentaAGuadar(cuentaNueva);
                    CuentaAGuardar.Usuario1 = ConvertirAJugador(cuentaNueva.Jugador);
                    Persistencia.CuentaSet.Add(CuentaAGuardar);
                    Persistencia.SaveChanges();
                    CuentaGuardada = ConvertirACuentaModel(CuentaAGuardar);
                    CuentaGuardada.Jugador = ConvertirAUsuarioModel(CuentaAGuardar.Usuario1);
                }
            }
            return CuentaGuardada;
        }

        /// <summary>
        /// Verifica si las credenciales ingresadas son validas para la cuenta ingresada
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>1 si las credenciales son validas, 0 si no, -1 si la cuenta no esta verificada</returns>
        public int IniciarSesion(CuentaModel Cuenta)
        {
            using (PersistenciaContainer Persistencia = new PersistenciaContainer())
            {
                int LoginValido = 0;
                Cuenta.Contrasena = Encriptador.ComputeSha256Hash(Cuenta.Contrasena);
                int CuentaExistente = Persistencia.CuentaSet.Where
                (cuentaEnDB => cuentaEnDB.Usuario == Cuenta.NombreUsuario
                               && cuentaEnDB.Password == Cuenta.Contrasena).Count();
                if (CuentaExistente == 1)
                {
                    if (Persistencia.CuentaSet.Where(cuentaEnDB => cuentaEnDB.Usuario == Cuenta.NombreUsuario
                                                                   && cuentaEnDB.Password == Cuenta.Contrasena && cuentaEnDB.Valida).Count() == 1)
                    {
                        LoginValido = 1;
                    }
                    else
                    {
                        LoginValido = -1;
                    }
                }
                return LoginValido;
            }
        }


        /// <summary>
        /// Cambia el estado de la cuenta de no verificada a verificada
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>Verdadero si la cuenta se verifico correctamente, falso si no</returns>
        public Boolean VerificarCuenta(CuentaModel Cuenta)
        {
            using (PersistenciaContainer Persistencia = new PersistenciaContainer())
            {
                Cuenta CuentaAVerificar = Persistencia.CuentaSet.Where(cuentaRecuperada =>
                    cuentaRecuperada.Usuario == Cuenta.NombreUsuario).FirstOrDefault();
                if (CuentaAVerificar != null)
                {
                    CuentaAVerificar.Valida = true;
                    Persistencia.SaveChanges();
                    return true;
                }
                return false;
            }
        }


        /// <summary>
        /// Cuenta el numero de Cuentas que tienen el mismo nombre de usuario
        /// </summary>
        /// <param name="NombreUsuario">String</param>
        /// <returns>La cantidad de usuarios repetidos</returns>
        private int ContarNombreDeUsuariosRepetidos(String NombreUsuario)
        {
            using (PersistenciaContainer Persistencia = new PersistenciaContainer())
            {
                return Persistencia.CuentaSet.Where
                    (cuenta => cuenta.Usuario == NombreUsuario).Count();
            }
        }

        /// <summary>
        /// Crea una cuenta a partir de un CuentaModel agregando los datos necesarios para 
        /// registrarla en la base de datos
        /// </summary>
        /// <param name="CuentaAConvertir">CuentaModel</param>
        /// <returns>Una Cuenta con la informacion necesaria para ser alamcenada en la base de datos </returns>
        private Cuenta CrearCuentaAGuadar(CuentaModel CuentaAConvertir)
        {
            return new Cuenta()
            {
                Usuario = CuentaAConvertir.NombreUsuario,
                Password = Encriptador.ComputeSha256Hash(CuentaAConvertir.Contrasena),
                CodigoVerificacion = GeneradorCodigo.GenerarCodigoActivacion(),
                Valida = false,
                CorreoElectronico = CuentaAConvertir.CorreoElectronico
            };
        }

        /// <summary>
        /// Convierte un objeto CuentaModel a Cuenta
        /// </summary>
        /// <param name="CuentaAConvertir">CuentaModel</param>
        /// <returns>La CuentaModel convertida a Cuenta</returns>
        private Cuenta ConvertirACuenta(CuentaModel CuentaAConvertir)
        {
            return new Cuenta()
            {
                Usuario = CuentaAConvertir.NombreUsuario,
                Password = CuentaAConvertir.Contrasena,
                Valida = CuentaAConvertir.Verificado
            };
        }

        /// <summary>
        /// Convierte un JugadorModel a Jugador
        /// </summary>
        /// <param name="JugadorAConvertir">JugadorModel</param>
        /// <returns>El JugadorModel convertido a JugadorModel</returns>
        private Jugador ConvertirAJugador(JugadorModel JugadorAConvertir)
        {
            Jugador JugadorConvertido;
            if(JugadorAConvertir != null)
            {
                JugadorConvertido= new Jugador()
                {
                    MejorPuntacion = JugadorAConvertir.MejorPuntacion,
                    UvCoins = JugadorAConvertir.UvCoins
                };
            }
            else
            {
                JugadorConvertido = new Jugador()
                {
                    MejorPuntacion = 0,
                    UvCoins = 0
                };
            }
            return JugadorConvertido;
        }

        /// <summary>
        /// Convierte un objeto Cuenta a CuentaModel
        /// </summary>
        /// <param name="CuentaAConvertir">Cuenta</param>
        /// <returns>La Cuenta convertida a CuentaModel</returns>
        private CuentaModel ConvertirACuentaModel(Cuenta CuentaAConvertir)
        {
            return new CuentaModel()
            {
                NombreUsuario = CuentaAConvertir.Usuario,
                Contrasena = CuentaAConvertir.Password,
                Verificado = CuentaAConvertir.Valida,
                CodigoVerificacion = CuentaAConvertir.CodigoVerificacion,
                CorreoElectronico = CuentaAConvertir.CorreoElectronico
            };
        }

        /// <summary>
        /// Convierte Jugador a JugadorModel
        /// </summary>
        /// <param name="UsuarioAConvertir">UsuarioAConvertir</param>
        /// <returns>El Juegador converitido a JugadorModel</returns>
        private JugadorModel ConvertirAUsuarioModel(Jugador UsuarioAConvertir)
        {
            return new JugadorModel()
            {
                MejorPuntacion = UsuarioAConvertir.MejorPuntacion,
                UvCoins = UsuarioAConvertir.UvCoins
            };
        }

        /// <summary>
        /// Convierte un CorredorAdquiridoModel a CorredorAdquirido
        /// </summary>
        /// <param name="PersonajeCorredorAConvertir">PersonajeCorredorAConvertir</param>
        /// <returns>El CorredorAdquieridoModel a CorredorAdquirido</returns>
        private CorredorAdquirido ConvertirCorredorAdquirido(CorredorAdquiridoModel PersonajeCorredorAConvertir)
        {
            return new CorredorAdquirido()
            {
                Nombre = PersonajeCorredorAConvertir.Nombre,
                Poder = PersonajeCorredorAConvertir.Poder,
                Precio = PersonajeCorredorAConvertir.Precio
            };
        }

        /// <summary>
        /// Convierte un objeto CorredorAdquirido a CorredorAdquiridoModel
        /// </summary>
        /// <param name="CorredorAdquirdoAConvertir">CorredorAdquirdoAConvertir</param>
        /// <returns>El CorredorAdquirido a CorredorAdquiridoModel</returns>
        private CorredorAdquiridoModel ConvertirCorredorAdquiridoModel(CorredorAdquirido CorredorAdquirdoAConvertir)
        {
            return new CorredorAdquiridoModel()
            {
                Nombre = CorredorAdquirdoAConvertir.Nombre,
                Poder = CorredorAdquirdoAConvertir.Poder,
                Precio = CorredorAdquirdoAConvertir.Precio
            };
        }

        /// <summary>
        /// Convierte un objeto PerseguidorAdquirido a SeguidorAdquiridoModel
        /// </summary>
        /// <param name="SeguidorAdquiridoAConvertir">CorredorAdquirido</param>
        /// <returns>El PerseguidorAdquirido a AdquiridoModel</returns>
        private SeguidorAdquiridoModel ConvertirSeguidorAdquiridoModel(PerseguidorAdquirido SeguidorAdquiridoAConvertir)
        {
            return new SeguidorAdquiridoModel()
            {
                Nombre = SeguidorAdquiridoAConvertir.Nombre,
                Precio = SeguidorAdquiridoAConvertir.Precio
            };
        }

        /// <summary>
        /// Convierte un objeto SeguidorAdquiridoModel a SeguirAdquirido
        /// </summary>
        /// <param name="SeguidorAdquirido">SeguidorAdquirido</param>
        /// <returns>El PersegudirAdquierodoModel convertido a PerseguidorAdquirido</returns>
        private PerseguidorAdquirido ConvertirSeguidorAdquirido(SeguidorAdquiridoModel SeguidorAdquirido)
        {
            return new PerseguidorAdquirido()
            {
                Nombre = SeguidorAdquirido.Nombre,
                Precio = SeguidorAdquirido.Precio
            };
        }

        /// <summary>
        /// Recupera la informacion completa de una cuenta en la base de datos
        /// </summary>
        /// <param name="CuentaARecuperar">String</param>
        /// <returns>CuentaModel o Null si la cuenta no existe</returns>
        public CuentaModel RecuperarCuenta(CuentaModel CuentaARecuperar)
        {
            using (PersistenciaContainer Persistencia = new PersistenciaContainer())
            {
                Cuenta CuentaRecuperada = Persistencia.CuentaSet.Where
                    (cuenta => cuenta.Usuario == CuentaARecuperar.NombreUsuario).FirstOrDefault();
                if (CuentaRecuperada != null)
                {
                    CuentaModel Cuenta = ConvertirACuentaModel(CuentaRecuperada);
                    JugadorModel Jugador = ConvertirAUsuarioModel(CuentaRecuperada.Usuario1);
                    List<CorredorAdquiridoModel> CorredoresAdquiridos = new List<CorredorAdquiridoModel>();
                    List<SeguidorAdquiridoModel> SeguidoresAdquiridos = new List<SeguidorAdquiridoModel>();
                    foreach (CorredorAdquirido Corredor in CuentaRecuperada.Usuario1.CorredoresAdquiridos)
                    {
                        CorredoresAdquiridos.Add(ConvertirCorredorAdquiridoModel(Corredor));
                    }
                    foreach (PerseguidorAdquirido Perseguidor in CuentaRecuperada.Usuario1.PerseguidorAdquirido)
                    {
                        SeguidoresAdquiridos.Add(ConvertirSeguidorAdquiridoModel(Perseguidor));
                    }
                    Jugador.CorredoresAdquiridos = CorredoresAdquiridos;
                    Jugador.SeguidoresAdquiridos = SeguidoresAdquiridos;
                    Cuenta.Jugador = Jugador;
                    return Cuenta;
                }
                return null;
            }
        }

        /// <summary>
        /// Actualiza los datos del Jugador de la cuenta
        /// </summary>
        /// <param name="CuentaActualizar">Cuenta a la que se actualizaran los datos</param>
        public void GuardarDatosDeLaCuenta(CuentaModel CuentaActualizar)
        {
            using (PersistenciaContainer Persistencia = new PersistenciaContainer())
            {

                Cuenta CuentaAModificar = Persistencia.CuentaSet.Where(cuenta => cuenta.Usuario == CuentaActualizar.NombreUsuario).FirstOrDefault();
                CuentaAModificar.Usuario1.UvCoins = CuentaActualizar.Jugador.UvCoins;
                CuentaAModificar.Usuario1.MejorPuntacion = CuentaActualizar.Jugador.MejorPuntacion;
                Persistencia.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene las primeras 5 CuentasConLasMejoresPuntuaciones
        /// </summary>
        /// <returns>Una lista de CuentaModel que tienen las mejores 5 puntuaciones</returns>
        public List<CuentaModel> RecuperarMejoresPuntuaciones()
        {
            List<CuentaModel> MayoresPuntuaciones = new List<CuentaModel>();
            using (PersistenciaContainer Persistencia = new PersistenciaContainer())
            {
                var CuentasDeMayoresPuntuaciones = Persistencia.CuentaSet.OrderByDescending(cuenta => cuenta.Usuario1.MejorPuntacion).Take(5);
                if (CuentasDeMayoresPuntuaciones != null)
                {
                    foreach (Cuenta CuentaRecuperada in CuentasDeMayoresPuntuaciones)
                    {
                        CuentaModel Cuenta = ConvertirACuentaModel(CuentaRecuperada);
                        JugadorModel Jugador = ConvertirAUsuarioModel(CuentaRecuperada.Usuario1);
                        List<CorredorAdquiridoModel> CorredoresAdquiridos = new List<CorredorAdquiridoModel>();
                        List<SeguidorAdquiridoModel> SeguidoresAdquiridos = new List<SeguidorAdquiridoModel>();
                        Cuenta.Jugador = Jugador;
                        MayoresPuntuaciones.Add(Cuenta);
                    }

                }
            }
            return MayoresPuntuaciones;
        }
    }
}
