using GameChatService.Contrato;
using GameChatService.Dominio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameChatService.Servicio
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple,
    UseSynchronizationContext = false)]
    public class ChatService:IChatService
    {
        Dictionary<Cuenta, IChatServiceCallback> cuentasConetadas = new Dictionary<Cuenta, IChatServiceCallback>();
        List<Cuenta> cuentas = new List<Cuenta>();
        Object sincronizarObjeto = new object();

        public IChatServiceCallback ActualCallback {
            get {
                return OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();
            }
        }


        /// <summary>
        /// Busca a un usuario por su nombre de usuario en el diccionario de usuarios conectados
        /// </summary>
        /// <param name="nombre">String</param>
        /// <returns>True si el usuario se encuentra, false si no</returns>
        private bool BuscarCuentaPorNombre(String nombre)
        {
            foreach (Cuenta cuenta in cuentasConetadas.Keys)
            {
                if (cuenta.usuario == nombre)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Agrega una Cuenta a la lista y diccionario de cuentas conectadas y le notifica a los usuariosConectados que alguien se ha conectado
        /// </summary>
        /// <param name="cuenta"></param>
        /// <returns>True si la cuenta se agrego correctamente y se notifico correctamente a los demas usuarios </returns>
        public bool Conectar(Cuenta cuenta)
        {
            Debug.WriteLine("La cuenta recibida es: " + cuenta.usuario);
            if (ActualCallback != null)
            {
                if (!cuentasConetadas.ContainsValue(ActualCallback) && !BuscarCuentaPorNombre(cuenta.usuario))
                {
                    lock (sincronizarObjeto)
                    {
                        cuentasConetadas.Add(cuenta, ActualCallback);
                        cuentas.Add(cuenta);
                        foreach (Cuenta cuentaClave in cuentasConetadas.Keys)
                        {
                            IChatServiceCallback callback = cuentasConetadas[cuentaClave];
                            Debug.WriteLine("Le estoy notificanfo al usuario " + cuentaClave.usuario);
                            try
                            {
                                callback.RefrescarCuentasConectadas(cuentas);
                                callback.Unirse(cuenta);
                            }
                            catch (Exception ex)
                            {
                                cuentasConetadas.Remove(cuenta);
                                Debug.Write("Ocurrio la excepcion: " + ex.Message);
                                throw new FaultException(ex.Message);
                                //return false;
                            }
                        }

                    }
                    Debug.Write("Se termino de sincronizar todos los hilos");
                        return true;
                    }
                }
                    return false;
        }

        public void Desconectar(Cuenta cuenta)
        {
            foreach (Cuenta cuentaClave in cuentasConetadas.Keys)
            {
                if (cuentaClave.usuario == cuentaClave.usuario)
                {
                    lock (sincronizarObjeto)
                    {
                        this.cuentasConetadas.Remove(cuentaClave);
                        this.cuentas.Remove(cuentaClave);
                        foreach (IChatServiceCallback callback in cuentasConetadas.Values)
                        {
                            callback.RefrescarCuentasConectadas(this.cuentas);
                            callback.Abandonar(cuenta);
                        }
                    }
                    return;
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
        /// <param name="cuenta">Cuenta</param>
        public void EstaEscribiendo(Cuenta cuenta)
        {
            lock (sincronizarObjeto)
            {
                foreach (IChatServiceCallback callback in cuentasConetadas.Values)
                {
                    callback.EstaEscribiendoCallback(cuenta);
                }
            }
        }
    }
}
