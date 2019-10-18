using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaDelNegocio.DataAccess.Interfaces;
using LogicaDelNegocio.Modelo;
using AccesoDatos;
using LogicaDelNegocio.Util;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace LogicaDelNegocio.DataAccess
{
    public class CuentaDAO : ICuentaDAO
    {
        /// <summary>
        /// Almacena en la base de datos una cuenta si el nombre de usuario no se repite
        /// </summary>
        /// <param name="cuentaNueva">CuentaModel</param>
        /// <returns> 1 si la cuenta se registro correctamente, 0 si la cuenta ya existe</returns>
        public CuentaModel CheckIn(CuentaModel cuentaNueva)
        {
            CuentaModel cuentaGuardada = null;
            try
            {
                using (PersistenciaContainer persistencia = new PersistenciaContainer())
                {
                    int usuariosRepetidos = ContarNombreDeUsuariosRepetidos(cuentaNueva.nombreUsuario);
                    if (usuariosRepetidos == 0)
                    {
                        Cuenta cuentaAGuardar = CrearCuentaAGuadar(cuentaNueva);
                        cuentaAGuardar.Usuario1 = ConvertirAUsuario(cuentaNueva.informacionDeUsuario);
                        cuentaAGuardar.Usuario1.Avance = new Avance();
                        persistencia.CuentaSet.Add(cuentaAGuardar);
                        persistencia.SaveChanges();
                        cuentaGuardada =  ConvertirACuentaModel(cuentaAGuardar);
                        cuentaGuardada.informacionDeUsuario = ConvertirAUsuarioModel(cuentaAGuardar.Usuario1);
                    }
                }
            }catch(EntityException)
            {
                throw;
            }
            return cuentaGuardada;
        }

        /// <summary>
        /// Verifica si las credenciales ingresadas son validas para la cuenta ingresada
        /// </summary>
        /// <param name="cuentaLogIn">CuentaModel</param>
        /// <returns>1 si las credenciales son validas, 0 si no, -1 si la cuenta no esta verificada</returns>
        public int LogIn(CuentaModel cuentaLogIn)
        {
            try
            {
                using (PersistenciaContainer persistencia = new PersistenciaContainer())
                {
                    int loginValido = 0;
                    cuentaLogIn.contrasena = Encriptador.ComputeSha256Hash(cuentaLogIn.contrasena);
                    int cuentaExistente = persistencia.CuentaSet.Where
                        (cuenta => cuenta.Usuario == cuentaLogIn.nombreUsuario
                        && cuenta.Password == cuentaLogIn.contrasena).Count();
                    if(cuentaExistente == 1)
                    {
                        if (persistencia.CuentaSet.Where(cuenta => cuenta.Usuario == cuentaLogIn.nombreUsuario
                        && cuenta.Password == cuentaLogIn.contrasena && cuenta.Valida).Count() == 1)
                        {
                            loginValido = 1;
                        }
                        else
                        {
                            loginValido = -1;
                        }
                    }
                    return loginValido;
                }
            }catch(EntityException )
            {
               throw;
            }
        }


        /// <summary>
        /// Cambia el estado de la cuenta de no verificada a verificada
        /// </summary>
        /// <param name="cuenta"></param>
        /// <returns>Bool</returns>
        public Boolean VerifyAccount(CuentaModel cuenta)
        {
            try
            {
                using (PersistenciaContainer persistencia = new PersistenciaContainer())
                {
                    Cuenta cuentaAVerificar = persistencia.CuentaSet.Where(cuentaRecuperada =>
                    cuentaRecuperada.Usuario == cuenta.nombreUsuario).FirstOrDefault();
                    if (cuentaAVerificar != null)
                    {
                        cuentaAVerificar.Valida = true;
                        persistencia.SaveChanges();
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
        /// <param name="nombreUsuario">String</param>
        /// <returns>La canitdad de usuarios repetidos</returns>
        private int ContarNombreDeUsuariosRepetidos(String nombreUsuario)
        {
            try
            {
                using (PersistenciaContainer persistencia = new PersistenciaContainer())
                {
                    return persistencia.CuentaSet.Where
                        (cuenta => cuenta.Usuario == nombreUsuario).Count();
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
        /// <param name="cuentaAConvertir">CuentaModel</param>
        /// <returns>Cuenta</returns>
        private Cuenta CrearCuentaAGuadar(CuentaModel cuentaAConvertir)
        {
            return new Cuenta()
            {
                Usuario = cuentaAConvertir.nombreUsuario,
                Password = Encriptador.ComputeSha256Hash(cuentaAConvertir.contrasena),
                CodigoVerificacion = GeneradorCodigo.GenerarCodigoActivacion(),
                Valida = false
            };
        }

        /// <summary>
        /// Convierte un objeto CuentaModel a Cuenta
        /// </summary>
        /// <param name="cuentaAConvertir">CuentaModel</param>
        /// <returns>Cuenta</returns>
        private Cuenta ConvertirACuenta(CuentaModel cuentaAConvertir)
        {
            return new Cuenta()
            {
                Usuario = cuentaAConvertir.nombreUsuario,
                Password = cuentaAConvertir.contrasena,
                Valida = cuentaAConvertir.verificado
            };
        }

        /// <summary>
        /// Convierte un objeto UsuarioModel a Usuario
        /// </summary>
        /// <param name="usuarioAConvertir">UsuarioModel</param>
        /// <returns>Usuario</returns>
        private Usuario ConvertirAUsuario(UsuarioModel usuarioAConvertir)
        {
            return new Usuario()
            {
                CorreoElectronico = usuarioAConvertir.correo,
                Edad = usuarioAConvertir.edad
            };
        }

        /// <summary>
        /// Convierte un objeto Cuenta a CuentaModel
        /// </summary>
        /// <param name="cuentaAConvertir">Cuenta</param>
        /// <returns>CuentaModel</returns>
        private CuentaModel ConvertirACuentaModel(Cuenta cuentaAConvertir)
        {
            return new CuentaModel()
            {
                nombreUsuario = cuentaAConvertir.Usuario,
                contrasena = cuentaAConvertir.Password,
                verificado = cuentaAConvertir.Valida,
                codigoVerificacion = cuentaAConvertir.CodigoVerificacion
            };
        }

        /// <summary>
        /// Convierte un objeto Usuario a UsuarioModel
        /// </summary>
        /// <param name="usuarioAConvertir">Usuario</param>
        /// <returns>UsuarioModel</returns>
        private UsuarioModel ConvertirAUsuarioModel(Usuario usuarioAConvertir)
        {
            return new UsuarioModel()
            {
                correo = usuarioAConvertir.CorreoElectronico,
                edad = Convert.ToString(usuarioAConvertir.Edad)
            };
        }

        /// <summary>
        /// Convierte un objeto Avance a AvanceModel
        /// </summary>
        /// <param name="avaneAConvertir">AvenceSet</param>
        /// <returns>AvanceModel</returns>
        private AvanceModel ConvertirAAvanceModel(Avance avaneAConvertir)
        {
            return new AvanceModel()
            {
                mejorPuntuacion = avaneAConvertir.MejorPuntuacion,
                uvCoins = avaneAConvertir.UvCoins
            };
        }

        /// <summary>
        /// Convierte un objeto PersonajeCorredor a PersonajeCorredorModel
        /// </summary>
        /// <param name="personajeCorredorAConvertir">PersonajeCorredor</param>
        /// <returns>PersonajeCorredorModel</returns>
        private PersonajeCorredorModel ConvertirAPersonaje(PersonajeCorredor personajeCorredorAConvertir)
        {
            return new PersonajeCorredorModel()
            {
                nombre = personajeCorredorAConvertir.Nombre,
                poder = personajeCorredorAConvertir.Poder,
                precio = personajeCorredorAConvertir.Precio
            };
        }

        /// <summary>
        /// Recupera la informacion completa de una cuenta en la base de datos
        /// </summary>
        /// <param name="cuentaARecuperar">String</param>
        /// <returns>CuentaModel o Null si la cuenta no existe</returns>
        public CuentaModel RecuperarCuenta(CuentaModel cuentaARecuperar)
        {
            try
            {
                using(PersistenciaContainer persistencia = new PersistenciaContainer())
                {
                    Cuenta cuentaRecuperada = persistencia.CuentaSet.Where
                        (cuenta => cuenta.Usuario == cuentaARecuperar.nombreUsuario).FirstOrDefault();
                    if(cuentaRecuperada != null)
                    {
                        CuentaModel cuenta = ConvertirACuentaModel(cuentaRecuperada);
                        UsuarioModel usuario = ConvertirAUsuarioModel(cuentaRecuperada.Usuario1);
                        AvanceModel avance = ConvertirAAvanceModel(cuentaRecuperada.Usuario1.Avance);
                        List<PersonajeCorredorModel> personajesAdquiridos = new List<PersonajeCorredorModel>();
                        foreach(PersonajeCorredor personaje in 
                            cuentaRecuperada.Usuario1.Avance.PersonajeCorredor)
                        {
                            personajesAdquiridos.Add(ConvertirAPersonaje(personaje));
                        }
                        avance.corredoresAdquiridos = personajesAdquiridos;
                        usuario.avanceDeUsuario = avance;
                        cuenta.informacionDeUsuario = usuario;
                        return cuenta;
                    }
                    return null;
                }
            }catch(EntityException ex)
            {
                throw;
            }
        }
    }
}
