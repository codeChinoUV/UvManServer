﻿using System;
using System.ServiceModel;
using LogicaDelNegocio.Util;
using LogicaDelNegocio.DataAccess.Interfaces;
using LogicaDelNegocio.DataAccess;
using System.Data.Entity.Core;
using LogicaDelNegocio.Modelo;
using CuentaService.Contrato;
using MessageService.Dominio.Enum;
using System.Net.Mail;

namespace CuentaService.Servicio
{
    /// <summary>
    /// Se encarga de proporcionar los servicios del juego
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple,
    UseSynchronizationContext = false)]
    public class CuentaService : ICuentaService
    {
        private ClienteCorreo ClienteCorreo = new ClienteCorreo();
        ICuentaDAO PersistenciaCuenta = new CuentaDAO();

        /// <summary>
        /// Envia el codigo de verifiacion al correo de la ceunta
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        public void ReEnviarCorreoVerificacion(CuentaModel Cuenta)
        {
            try
            {
                CuentaModel CuentaAReEnviarCorreoCompleta = PersistenciaCuenta.RecuperarCuenta(Cuenta);
                EnviarCorreoDeVerificacion(Cuenta);
            }
            catch (SmtpException)
            {
                
            }
        }

        /// <summary>
        /// Guarda un User en la base de datos si el nombre de usuario no existe en la base de datos.
        /// </summary>
        /// <param name="CuentaNueva">CuentaModel</param>
        /// <returns>EnumEstadoRegistro</returns>
        public EnumEstadoRegistro Registrarse(CuentaModel CuentaNueva)
        {
            EnumEstadoRegistro EstadoDelRegistro = EnumEstadoRegistro.UsuarioExistente;
            if (CuentaNueva != null)
            {
                try
                {
                    CuentaModel CuentaAlmacenda = PersistenciaCuenta.Registrarse(CuentaNueva);
                    if (CuentaAlmacenda != null)
                    {
                        EnviarCorreoDeVerificacion(CuentaAlmacenda);
                        EstadoDelRegistro = EnumEstadoRegistro.RegistroCorrecto;
                    }
                }
                catch (EntityException)
                {
                    EstadoDelRegistro = EnumEstadoRegistro.ErrorEnBaseDatos; 
                }
            }
            return EstadoDelRegistro;
        }

        /// <summary>
        /// Verifica si el codigo introducido para la cuenta coincide con el de la cuenta
        /// </summary>
        /// <param name="CodigoDeVerificacion">String</param>
        /// <param name="CuentaAVerficar">CuentaModel</param>
        /// <returns>EnumEstadoVerificarCuenta</returns>
        public EnumEstadoVerificarCuenta VerificarCuenta(String CodigoDeVerificacion, CuentaModel CuentaAVerficar)
        {
            EnumEstadoVerificarCuenta EstadoVerificacion = EnumEstadoVerificarCuenta.NoCoincideElCodigo;
            try
            {
                CuentaModel CuentaAVerificarCompleta = PersistenciaCuenta.RecuperarCuenta(CuentaAVerficar);
                if (CuentaAVerificarCompleta.VerificarCuenta(CodigoDeVerificacion))
                {
                    if (PersistenciaCuenta.VerificarCuenta(CuentaAVerificarCompleta))
                    {
                        EstadoVerificacion = EnumEstadoVerificarCuenta.VerificadaCorrectamente;
                    }
                    else
                    {
                        EstadoVerificacion = EnumEstadoVerificarCuenta.ErrorEnBaseDatos;
                    }
                }
            }
            catch (EntityException)
            {
                EstadoVerificacion = EnumEstadoVerificarCuenta.ErrorEnBaseDatos;
            }
            return EstadoVerificacion;
        }

        /// <summary>
        /// Envia un correo a la Cuenta con su codigo de verificación
        /// </summary>
        /// <param name="CuentaAEnviar"></param>
        /// <returns>Verdadero si se envio el correo de verificacion, falso si no</returns>
        private Boolean EnviarCorreoDeVerificacion(CuentaModel CuentaAEnviar)
        {
            String Contenido = ClienteCorreo.GenerarContenidoVerificacion(CuentaAEnviar.CodigoVerificacion);
            String Asunto = "Codigo de verificación del correo electronico";
            return ClienteCorreo.EnviarCorreoHtml(CuentaAEnviar.CorreoElectronico, Asunto, Contenido);
        }


    }
}
