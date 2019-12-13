using GameChatService.Contrato;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using GameService.Dominio;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.Util;
using Message = GameChatService.Dominio.Message;


namespace GameChatService.Servicio
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple,
    UseSynchronizationContext = false)]
    public class ChatService:IChatService
    {
        Dictionary<CuentaModel, IChatServiceCallback> CuentasConetadas = new Dictionary<CuentaModel, IChatServiceCallback>();
        readonly List<CuentaModel> Cuentas = new List<CuentaModel>();
        readonly Object SincronizarObjeto = new object();
        SalaManager ManejadorDeSalas = SalaManager.GetSalaManager();
        

        public IChatServiceCallback ActualCallback {
            get {
                return OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();
            }
        }


        /// <summary>
        /// Busca a un usuario por su nombre de usuario en el diccionario de usuarios conectados
        /// </summary>
        /// <param name="Nombre">String</param>
        /// <returns>Boolean</returns>
        private bool ExisteCuenta(String Nombre)
        {
            lock (SincronizarObjeto)
            {
                foreach (CuentaModel Cuenta in CuentasConetadas.Keys)
                {
                    if (Cuenta.NombreUsuario == Nombre)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Agrega una Cuenta a la lista y diccionario de cuentas conectadas y le notifica a los usuariosConectados de la nueva
        /// cuenta conectada
        /// </summary>
        /// <param name="Cuenta"></param>
        /// <returns>Boolean</returns>
        public bool Conectar(CuentaModel Cuenta)
        {
            SessionManager ManejadorDeSesiones = SessionManager.GetSessionManager();
            if (ActualCallback != null)
            {
                ManejadorDeSesiones.UsuarioDesconectado += CerroSesionGlobal;
                lock (SincronizarObjeto)
                {
                    if (!CuentasConetadas.ContainsValue(ActualCallback) && !ExisteCuenta(Cuenta.NombreUsuario) && 
                        ManejadorDeSesiones.VerificarCuentaLogeada(Cuenta))
                    {
                        lock (SincronizarObjeto)
                        {
                            NotificarClientesNuevoConectado(Cuenta);
                            CuentasConetadas.Add(Cuenta, ActualCallback);
                        }
                        Cuentas.Add(Cuenta);
                        return true;
                    }
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
            return Cuentas;
        }

        /// <summary>
        /// Termina la sesion de una cuenta en el servicio de chat
        /// </summary>
        /// <param name="Cuenta">Cuenta</param>
        public void Desconectar(CuentaModel Cuenta)
        {
            CuentaModel CuentaCompleta = ObtenerCuentaEnElDiccionario(Cuenta);
            lock (SincronizarObjeto)
            {
                List<CuentaModel> CuentasDeLaSala = ManejadorDeSalas.RecuperarCuentasDeSalaDeJugador((Cuenta));
                foreach (CuentaModel CuentaClave in CuentasConetadas.Keys)
                {
                    foreach (CuentaModel CuentaEnSala in CuentasDeLaSala)
                    {
                        if (CuentaClave.NombreUsuario == CuentaEnSala.NombreUsuario)
                        {
                            IChatServiceCallback Callback = CuentasConetadas[CuentaClave];
                            Callback.Abandonar(Cuenta);        
                        }
                    }       
                }
                if (CuentaCompleta != null)
                {
                    CuentasConetadas.Remove(CuentaCompleta);
                    Cuentas.Remove(CuentaCompleta);
                }
            }
        }

        /// <summary>
        /// Notifica a las demas cuentas del mensaje enviado
        /// </summary>
        /// <param name="Mensaje">Message</param>
        public void EnviarMensaje(Message Mensaje)
        {
            List<CuentaModel> CuentasEnSala = ManejadorDeSalas.RecuperarCuentasDeSalaDeJugador(Mensaje.Remitente);
            Debug.WriteLine(CuentasEnSala.Count);

            lock (SincronizarObjeto)
            {
                foreach (CuentaModel CuentaEnSala in CuentasEnSala)
                {
                    foreach(CuentaModel CuentaClave in CuentasConetadas.Keys)
                    {
                        if(CuentaEnSala.NombreUsuario == CuentaClave.NombreUsuario)
                        {
                            Debug.WriteLine("Se esta notificando a " + CuentaClave.NombreUsuario);
                            IChatServiceCallback callback = CuentasConetadas[CuentaClave];
                            callback.RecibirMensaje(Mensaje);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recorre el diccionario de CuentasModel conectados, notificando que la cuenta se ha conectado
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        /// <returns>Boolean</returns>
        private Boolean NotificarClientesNuevoConectado(CuentaModel Cuenta)
        {
            SessionManager ManejadorDeSesiones = SessionManager.GetSessionManager();
            List<CuentaModel> CuentasDeSalaDeJugador = ManejadorDeSalas.RecuperarCuentasDeSalaDeJugador(Cuenta);
            foreach(CuentaModel cuentaDeSala in CuentasDeSalaDeJugador)
            {
                foreach (CuentaModel CuentaClave in CuentasConetadas.Keys)
                {
                    if(CuentaClave.NombreUsuario == cuentaDeSala.NombreUsuario)
                    {
                        if (ManejadorDeSesiones.VerificarCuentaLogeada(CuentaClave))
                        {
                            IChatServiceCallback Callback = CuentasConetadas[CuentaClave];
                            try
                            {
                                Callback.Unirse(Cuenta);
                            }
                            catch (Exception)
                            {
                                CuentasConetadas.Remove(Cuenta);
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Cierra la sesion en el chat si se cerro sesion global y no se cerro en el chat
        /// </summary>
        /// <param name="Cuenta">CuentaModel</param>
        private void CerroSesionGlobal(CuentaModel Cuenta)
        {
            if (ExisteCuenta(Cuenta.NombreUsuario))
            {
                Desconectar(Cuenta);
            }
        }

        /// <summary>
        /// Recupera del diccionario de listas conectadas la cuenta que tenga el mismo nombre que la parcial que se paso por parametro
        /// </summary>
        /// <param name="Parcial">CuentaModel</param>
        /// <returns>CuentaModel</returns>
        private CuentaModel ObtenerCuentaEnElDiccionario(CuentaModel Parcial)
        {
            lock (SincronizarObjeto)
            {
                foreach (CuentaModel CuentaClave in CuentasConetadas.Keys)
                {
                    if (CuentaClave.NombreUsuario == Parcial.NombreUsuario)
                    {
                        return CuentaClave;
                    }
                }
            }

            return null;
        }
        //Solo se modifico para que resivieran solo en sus salas el enviar mesaje y el conectarse
    }
}
