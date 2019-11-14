using GameChatService.Contrato;
using GameChatService.Dominio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.Util;

namespace GameChatService.Servicio
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple,
    UseSynchronizationContext = false)]

    public class ChatService:IChatService
    {
        Dictionary<CuentaModel, IChatServiceCallback> cuentasConetadas = new Dictionary<CuentaModel, IChatServiceCallback>();
        readonly List<CuentaModel> cuentas = new List<CuentaModel>();
        readonly Object sincronizarObjeto = new object();

        public IChatServiceCallback ActualCallback {
            get {
                return OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();
            }
        }


        /// <summary>
        /// Busca a un usuario por su nombre de usuario en el diccionario de usuarios conectados
        /// </summary>
        /// <param name="nombre">String</param>
        /// <returns>Boolean</returns>
        private bool ExisteCuenta(String nombre)
        {
            foreach (CuentaModel cuenta in cuentasConetadas.Keys)
            {
                if (cuenta.nombreUsuario == nombre)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Agrega una Cuenta a la lista y diccionario de cuentas conectadas y le notifica a los usuariosConectados de la nueva
        /// cuenta conectada
        /// </summary>
        /// <param name="cuenta"></param>
        /// <returns>Boolean</returns>
        public bool Conectar(CuentaModel cuenta)
        {
            SessionManager manejadorDeSesiones = SessionManager.GetSessionManager();
            if (ActualCallback != null)
            {
                manejadorDeSesiones.UsuarioDesconectado += CerroSesionGlobal;
                if (!cuentasConetadas.ContainsValue(ActualCallback) && !ExisteCuenta(cuenta.nombreUsuario) && 
                    manejadorDeSesiones.VerificarCuentaLogeada(cuenta))
                {
                    lock (sincronizarObjeto)
                    {
                        NotificarClientesNuevoConectado(cuenta);
                    }
                    cuentasConetadas.Add(cuenta, ActualCallback);
                    cuentas.Add(cuenta);
                    Debug.Write("Se termino de sincronizar todos los hilos");
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Regresa la lista de usuarios conectados
        /// </summary>
        /// <returns>List</returns>
        public List<CuentaModel> ObtenerCuentasConectadas()
        {
            return cuentas;
        }

        /// <summary>
        /// Termina la sesion de una cuenta en el servicio de chat
        /// </summary>
        /// <param name="cuenta">Cuenta</param>
        public void Desconectar(CuentaModel cuenta)
        {
            CuentaModel cuentaClave = ObtenerCuentaEnElDiccionario(cuenta);
            lock (sincronizarObjeto)
            {
                if(cuentaClave != null)
                {
                    cuentasConetadas.Remove(cuentaClave);
                    cuentas.Remove(cuentaClave);
                }
                foreach (IChatServiceCallback callback in cuentasConetadas.Values)
                {
                    callback.RefrescarCuentasConectadas(cuentas);
                    callback.Abandonar(cuenta);
                }
            }
        }

        /// <summary>
        /// Notifica a las demas cuentas del mensaje enviado
        /// </summary>
        /// <param name="mensaje">Message</param>
        public void EnviarMensaje(Message mensaje)
        {
            lock (sincronizarObjeto)
            {
                foreach (IChatServiceCallback callback in cuentasConetadas.Values)
                {
                    callback.RecibirMensaje(mensaje);
                }
            }
        }

        /// <summary>
        /// Notifica a las cuentas que una cuenta esta escribiendo
        /// </summary>
        /// <param name="cuenta">String</param>
        public void EstaEscribiendo(String cuenta)
        {
            lock (sincronizarObjeto)
            {
                foreach (IChatServiceCallback callback in cuentasConetadas.Values)
                {
                    callback.EstaEscribiendoCallback(cuenta);
                }
            }
        }

        /// <summary>
        /// Recorre el diccionario de CuentasModel conectados, notificando que la cuenta se ha conectado
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        /// <returns>Boolean</returns>
        private Boolean NotificarClientesNuevoConectado(CuentaModel cuenta)
        {
            SessionManager manejadorDeSesiones = SessionManager.GetSessionManager();
            foreach (CuentaModel cuentaClave in cuentasConetadas.Keys)
            {
                if (manejadorDeSesiones.VerificarCuentaLogeada(cuentaClave))
                {
                    IChatServiceCallback callback = cuentasConetadas[cuentaClave];
                    try
                    {
                        callback.RefrescarCuentasConectadas(cuentas);
                        callback.Unirse(cuenta);
                    }
                    catch (Exception ex)
                    {
                        cuentasConetadas.Remove(cuenta);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Cierra la sesion en el chat si se cerro sesion global y no se cerro en el chat
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        private void CerroSesionGlobal(CuentaModel cuenta)
        {
            if (ExisteCuenta(cuenta.nombreUsuario))
            {
                Desconectar(cuenta);
            }
        }

        /// <summary>
        /// Recupera del diccionario de listas conectadas la cuenta que tenga el mismo nombre que la parcial que se paso por parametro
        /// </summary>
        /// <param name="parcial">CuentaModel</param>
        /// <returns>CuentaModel</returns>
        private CuentaModel ObtenerCuentaEnElDiccionario(CuentaModel parcial)
        {
            foreach (CuentaModel cuentaClave in cuentasConetadas.Keys)
            {
                if (cuentaClave.nombreUsuario == parcial.nombreUsuario)
                {
                    return cuentaClave;
                }
            }
            return null;
        }
    }
}
