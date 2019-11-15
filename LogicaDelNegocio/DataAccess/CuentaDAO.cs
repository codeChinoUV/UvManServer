using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using LogicaDelNegocio.DataAccess.Interfaces;
using LogicaDelNegocio.Modelo;
using AccesoDatos;
using LogicaDelNegocio.Util;
using System.Diagnostics;

namespace LogicaDelNegocio.DataAccess
{
    public class CuentaDAO : ICuentaDAO
    {
        /// <summary>
        /// Almacena en la base de datos una cuenta si el nombre de usuario no se repite
        /// </summary>
        /// <param name="cuentaNueva">CuentaModel</param>
        /// <returns>CuentaModel</returns>
        public CuentaModel Registrarse(CuentaModel cuentaNueva)
        {
            CuentaModel CuentaGuardada = null;
            try
            {
                using (PersistenciaContainer Persistencia = new PersistenciaContainer())
                {
                    int UsuariosRepetidos = ContarNombreDeUsuariosRepetidos(cuentaNueva.NombreUsuario);
                    if (UsuariosRepetidos == 0)
                    {
                        Cuenta CuentaAGuardar = CrearCuentaAGuadar(cuentaNueva);
                        CuentaAGuardar.Usuario1 = ConvertirAUsuario(cuentaNueva.InformacionDeUsuario);
                        CuentaAGuardar.Usuario1.Avance = new Avance();
                        Persistencia.CuentaSet.Add(CuentaAGuardar);
                        Persistencia.SaveChanges();
                        CuentaGuardada =  ConvertirACuentaModel(CuentaAGuardar);
                        CuentaGuardada.InformacionDeUsuario = ConvertirAUsuarioModel(CuentaAGuardar.Usuario1);
                    }
                }
            }catch(EntityException)
            {
                throw;
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
            try
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
            }catch(EntityException)
            {
               throw;
            }
        }


        /// <summary>
        /// Cambia el estado de la cuenta de no verificada a verificada
        /// </summary>
        /// <param name="Cuenta"></param>
        /// <returns>Boolean</returns>
        public Boolean VerificarCuenta(CuentaModel Cuenta)
        {
            try
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
                    else
                    {
                        return false;
                    }
                }
            }
            catch (EntityException)
            {
                throw;
            }
            
        }


        /// <summary>
        /// Cuenta el numero de Cuentas que tienen el mismo nombre de usuario
        /// </summary>
        /// <param name="NombreUsuario">String</param>
        /// <returns>La canitdad de usuarios repetidos</returns>
        private int ContarNombreDeUsuariosRepetidos(String NombreUsuario)
        {
            try
            {
                using (PersistenciaContainer Persistencia = new PersistenciaContainer())
                {
                    return Persistencia.CuentaSet.Where
                        (cuenta => cuenta.Usuario == NombreUsuario).Count();
                }
            }
            catch (EntityException)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Crea una cuenta a partir de un CuentaModel agregando los datos necesarios para 
        /// registrarla en la base de datos
        /// </summary>
        /// <param name="CuentaAConvertir">CuentaModel</param>
        /// <returns>Cuenta</returns>
        private Cuenta CrearCuentaAGuadar(CuentaModel CuentaAConvertir)
        {
            return new Cuenta()
            {
                Usuario = CuentaAConvertir.NombreUsuario,
                Password = Encriptador.ComputeSha256Hash(CuentaAConvertir.Contrasena),
                CodigoVerificacion = GeneradorCodigo.GenerarCodigoActivacion(),
                Valida = false
            };
        }

        /// <summary>
        /// Convierte un objeto CuentaModel a Cuenta
        /// </summary>
        /// <param name="CuentaAConvertir">CuentaModel</param>
        /// <returns>Cuenta</returns>
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
        /// Convierte un objeto UsuarioModel a Usuario
        /// </summary>
        /// <param name="UsuarioAConvertir">UsuarioModel</param>
        /// <returns>Usuario</returns>
        private Usuario ConvertirAUsuario(UsuarioModel UsuarioAConvertir)
        {
            return new Usuario()
            {
                CorreoElectronico = UsuarioAConvertir.Correo,
                Edad = UsuarioAConvertir.Edad
            };
        }

        /// <summary>
        /// Convierte un objeto Cuenta a CuentaModel
        /// </summary>
        /// <param name="CuentaAConvertir">Cuenta</param>
        /// <returns>CuentaModel</returns>
        private CuentaModel ConvertirACuentaModel(Cuenta CuentaAConvertir)
        {
            return new CuentaModel()
            {
                NombreUsuario = CuentaAConvertir.Usuario,
                Contrasena = CuentaAConvertir.Password,
                Verificado = CuentaAConvertir.Valida,
                CodigoVerificacion = CuentaAConvertir.CodigoVerificacion
            };
        }

        /// <summary>
        /// Convierte un objeto Usuario a UsuarioModel
        /// </summary>
        /// <param name="UsuarioAConvertir">Usuario</param>
        /// <returns>UsuarioModel</returns>
        private UsuarioModel ConvertirAUsuarioModel(Usuario UsuarioAConvertir)
        {
            return new UsuarioModel()
            {
                Correo = UsuarioAConvertir.CorreoElectronico,
                Edad = Convert.ToString(UsuarioAConvertir.Edad)
            };
        }

        /// <summary>
        /// Convierte un objeto Avance a AvanceModel
        /// </summary>
        /// <param name="AvaneAConvertir">AvenceSet</param>
        /// <returns>AvanceModel</returns>
        private AvanceModel ConvertirAAvanceModel(Avance AvaneAConvertir)
        {
            return new AvanceModel()
            {
                MejorPuntuacion = AvaneAConvertir.MejorPuntuacion,
                UvCoins = AvaneAConvertir.UvCoins
            };
        }

        /// <summary>
        /// Convierte un objeto PersonajeCorredor a PersonajeCorredorModel
        /// </summary>
        /// <param name="PersonajeCorredorAConvertir">PersonajeCorredor</param>
        /// <returns>PersonajeCorredorModel</returns>
        private PersonajeCorredorModel ConvertirAPersonaje(PersonajeCorredor PersonajeCorredorAConvertir)
        {
            return new PersonajeCorredorModel()
            {
                Nombre = PersonajeCorredorAConvertir.Nombre,
                Poder = PersonajeCorredorAConvertir.Poder,
                Precio = PersonajeCorredorAConvertir.Precio
            };
        }

        /// <summary>
        /// Recupera la informacion completa de una cuenta en la base de datos
        /// </summary>
        /// <param name="CuentaARecuperar">String</param>
        /// <returns>CuentaModel o Null si la cuenta no existe</returns>
        public CuentaModel RecuperarCuenta(CuentaModel CuentaARecuperar)
        {
            try
            {
                using(PersistenciaContainer Persistencia = new PersistenciaContainer())
                {
                    Cuenta CuentaRecuperada = Persistencia.CuentaSet.Where
                        (cuenta => cuenta.Usuario == CuentaARecuperar.NombreUsuario).FirstOrDefault();
                    if(CuentaRecuperada != null)
                    {
                        CuentaModel Cuenta = ConvertirACuentaModel(CuentaRecuperada);
                        UsuarioModel Usuario = ConvertirAUsuarioModel(CuentaRecuperada.Usuario1);
                        AvanceModel Avance = ConvertirAAvanceModel(CuentaRecuperada.Usuario1.Avance);
                        List<PersonajeCorredorModel> PersonajesAdquiridos = new List<PersonajeCorredorModel>();
                        foreach(PersonajeCorredor Personaje in 
                            CuentaRecuperada.Usuario1.Avance.PersonajeCorredor)
                        {
                            PersonajesAdquiridos.Add(ConvertirAPersonaje(Personaje));
                        }
                        Avance.CorredoresAdquiridos = PersonajesAdquiridos;
                        Usuario.AvanceDeUsuario = Avance;
                        Cuenta.InformacionDeUsuario = Usuario;
                        return Cuenta;
                    }
                    return null;
                }
            }catch(EntityException)
            {
                throw;
            }
        }
    }
}
