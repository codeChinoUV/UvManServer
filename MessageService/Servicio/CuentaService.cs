using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using LogicaDelNegocio;
using LogicaDelNegocio.Util;
using LogicaDelNegocio.DataAccess.Interfaces;
using LogicaDelNegocio.DataAccess;
using System.Data.Entity.Core;
using LogicaDelNegocio.Modelo;
using CuentaService.Contrato;
using CuentaService.Dominio;

namespace CuentaService.Servicio
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple,
    UseSynchronizationContext = false)]
    public class CuentaService : ICuentaService
    {
        private ClienteCorreo clienteCorreo = new ClienteCorreo();
        ICuentaDAO persistenciaCuenta = new CuentaDAO();

        /// <summary>
        /// Guarda un User en la base de datos si el nombre de usuario no existe en la base de datos.
        /// </summary>
        /// <param name="newUser">User</param>
        /// <returns> 1 si el usuario se registro correctamente
        /// 0 si el usuario ya existe en la base de datos
        /// -1 si ocurrio algun error con la base de datos </returns>
        public int CheckIn(Cuenta nuevaCuenta)
        {
            int estadoRegistro = 0;
            if (nuevaCuenta != null)
            {
                try
                {
                    CuentaModel cuenta = ConvertirACuentaModel(nuevaCuenta);
                    cuenta.informacionDeUsuario = ConvertirAUsuarioModel(nuevaCuenta.datosDelUsuario);
                    CuentaModel cuentaAlmacenda = persistenciaCuenta.CheckIn(cuenta);
                    if (cuentaAlmacenda != null)
                    {
                        EnviarCorreoDeVerificacion(cuentaAlmacenda);
                        estadoRegistro = 1;
                    }
                }
                catch (EntityException)
                {
                    estadoRegistro = -1;
                }
            }
            return estadoRegistro;
        }

        /// <summary>
        /// Verifica si el codigo introducido para la cuenta coincide con el de la cuenta
        /// </summary>
        /// <param name="code">String</param>
        /// <param name="cuenta">Cuenta</param>
        /// <returns>1 si la cuenta se verifico correctamente, 0 si no coinciden los codigos, -1 ocurrio un error</returns>
        public int VerifyAccount(String code, Cuenta cuenta)
        {
            int estadoVerificacion = 0;
            CuentaModel cuentaModelARecuperar = ConvertirACuentaModel(cuenta);
            try
            {
                CuentaModel cuentaAVerificar = persistenciaCuenta.RecuperarCuenta(cuentaModelARecuperar);
                if (cuentaAVerificar.VerificarCuenta(code))
                {
                    if (persistenciaCuenta.VerifyAccount(cuentaAVerificar))
                    {
                        estadoVerificacion = 1;
                    }
                    else
                    {
                        estadoVerificacion = -1;
                    }
                }
            }
            catch (EntityException)
            {
                estadoVerificacion = -1;
            }
            return estadoVerificacion;
        }

        /// <summary>
        /// Verifica que el nombre de usuario y la contraseña se encuentren en la base de datos y 
        /// logea la cuenta en el sistema
        /// </summary>
        /// <param name="cuenta">Cuenta</param>
        /// <returns>1 si se logeo correctamente, 0 si las credenciales no son validas o -1 si ocurrio un error</returns>
        //public int LogIn(Cuenta cuenta)
        //{
        //    CuentaModel cuentaIniciarSesion = ConvertirACuentaModel(cuenta);
        //    int existeCuenta = persistenciaCuenta.LogIn(cuentaIniciarSesion);
        //    if (existeCuenta == 1)
        //    {
        //        CuentaModel cuentaRecuperada = persistenciaCuenta.RecuperarCuenta(cuentaIniciarSesion);
        //        ManejoSesion sesiones = ManejoSesion.Instancia();
        //        Boolean sesionCreada = sesiones.CrearSesion(cuentaRecuperada);
        //        if (sesionCreada)
        //        {
        //            return 1;
        //        }
        //        return -1;
        //    }
        //    return existeCuenta;

        //}

        /// <summary>
        /// Termina la sesion de la cuenta en el servidor
        /// </summary>
        /// <param name="cuenta">Cuenta</param>
        /// <returns>1 si se cerro correctamente, -1 si no</returns>
        //public int LogOut(Cuenta cuenta)
        //{
        //    CuentaModel cuentaATerminar = ConvertirACuentaModel(cuenta);
        //    if (ManejoSesion.Instancia().TerminarSesion(cuentaATerminar))
        //    {
        //        return 1;
        //    }
        //    return -1;
        //}

        /// <summary>
        /// Convierte una Cuenta en una CuentaModel
        /// </summary>
        /// <param name="cuentaConvertir">CuentaModel</param>
        /// <returns>Cuenta</returns>
        private CuentaModel ConvertirACuentaModel(Cuenta cuentaConvertir)
        {
            return new CuentaModel()
            {
                nombreUsuario = cuentaConvertir.usuario,
                contrasena = cuentaConvertir.contrasena,
                verificado = cuentaConvertir.verificada,
                codigoVerificacion = cuentaConvertir.codigoVerificacion
            };
        }

        /// <summary>
        /// Convierte un Usuaro en UsuarioModel
        /// </summary>
        /// <param name="usuarioConvertir">UsuarioModel</param>
        /// <returns>Usuario</returns>
        private UsuarioModel ConvertirAUsuarioModel(Usuario usuarioConvertir)
        {
            return new UsuarioModel()
            {
                correo = usuarioConvertir.correo,
                edad = usuarioConvertir.edad
            };
        }

        /// <summary>
        /// Envia un correo a la Cuenta con su codigo de verificación
        /// </summary>
        /// <param name="cuentaAVerificar"></param>
        /// <returns></returns>
        private Boolean EnviarCorreoDeVerificacion(CuentaModel cuentaAVerificar)
        {
            String contenido = ClienteCorreo.GenerarContenidoVerificacion(cuentaAVerificar.codigoVerificacion);
            String asunto = "Codigo de verificación del correo electronico";
            return clienteCorreo.EnviarCorreoHtml(cuentaAVerificar.informacionDeUsuario.correo, asunto, contenido);
        }


    }
}
