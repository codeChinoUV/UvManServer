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
using SessionService.Dominio.Enum;
using MessageService.Dominio.Enum;

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
        /// <param name="newUser">CuentaModel</param>
        /// <returns>EnumEstadoRegistro</returns>
        public EnumEstadoRegistro CheckIn(CuentaModel nuevaCuenta)
        {
            EnumEstadoRegistro estadoDelRegistro = EnumEstadoRegistro.UsuarioExistente;
            if (nuevaCuenta != null)
            {
                try
                {
                    CuentaModel cuentaAlmacenda = persistenciaCuenta.Registrarse(nuevaCuenta);
                    if (cuentaAlmacenda != null)
                    {
                        EnviarCorreoDeVerificacion(cuentaAlmacenda);
                        estadoDelRegistro = EnumEstadoRegistro.RegistroCorrecto;
                    }
                }
                catch (EntityException)
                {
                    estadoDelRegistro = EnumEstadoRegistro.ErrorEnBaseDatos; 
                }
            }
            return estadoDelRegistro;
        }

        /// <summary>
        /// Verifica si el codigo introducido para la cuenta coincide con el de la cuenta
        /// </summary>
        /// <param name="code">String</param>
        /// <param name="cuenta">CuentaModel</param>
        /// <returns>EnumEstadoVerificarCuenta</returns>
        public EnumEstadoVerificarCuenta VerifyAccount(String code, CuentaModel cuenta)
        {
            EnumEstadoVerificarCuenta estadoVerificacion = EnumEstadoVerificarCuenta.NoCoincideElCodigo;
            try
            {
                CuentaModel cuentaAVerificar = persistenciaCuenta.RecuperarCuenta(cuenta);
                if (cuentaAVerificar.VerificarCuenta(code))
                {
                    if (persistenciaCuenta.VerificarCuenta(cuentaAVerificar))
                    {
                        estadoVerificacion = EnumEstadoVerificarCuenta.VerificadaCorrectamente;
                    }
                    else
                    {
                        estadoVerificacion = EnumEstadoVerificarCuenta.ErrorEnBaseDatos;
                    }
                }
            }
            catch (EntityException)
            {
                estadoVerificacion = EnumEstadoVerificarCuenta.ErrorEnBaseDatos;
            }
            return estadoVerificacion;
        }

        /// <summary>
        /// Envia un correo a la Cuenta con su codigo de verificación
        /// </summary>
        /// <param name="cuentaAVerificar"></param>
        /// <returns>Boolean</returns>
        private Boolean EnviarCorreoDeVerificacion(CuentaModel cuentaAVerificar)
        {
            String contenido = ClienteCorreo.GenerarContenidoVerificacion(cuentaAVerificar.codigoVerificacion);
            String asunto = "Codigo de verificación del correo electronico";
            return clienteCorreo.EnviarCorreoHtml(cuentaAVerificar.informacionDeUsuario.correo, asunto, contenido);
        }


    }
}
