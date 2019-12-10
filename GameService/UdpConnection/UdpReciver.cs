using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameService.Dominio
{
    /// <summary>
    /// Se encarfa de recibir paquetes UDP
    /// </summary>
    public class UdpReciver
    {
        private UdpClient ClienteUDP;
        
        public delegate void RecibirEventoEnJuego(EventoEnJuego eventoEnJuego);
        public event RecibirEventoEnJuego EventoRecibido;

        /// <summary>
        /// Deseariliza un arreglo de bytes en un EventoEnJuego
        /// </summary>
        /// <param name="byteArray">byte[]</param>
        /// <returns>El EventoEnJuego deserializado</returns>
        public static EventoEnJuego Deserializar(byte[] byteArray)
        {
            if (byteArray == null)
            {
                return null;
            }
            BinaryFormatter FormateadorBinario = new BinaryFormatter();
            using(MemoryStream StreamDeMemoria = new MemoryStream())
            {
                StreamDeMemoria.Write(byteArray, 0, byteArray.Length);
                StreamDeMemoria.Seek(0, SeekOrigin.Begin);
                EventoEnJuego eventoEnJuego = (EventoEnJuego)FormateadorBinario.Deserialize(StreamDeMemoria);
                return eventoEnJuego;
            }
        }

        /// <summary>
        /// Se encarga de escuhar en la red a la espera de paquetes UDP
        /// </summary>
        /// <param name="puerto">Int</param>
        public void RecibirDatos(object puerto)
        {
            ClienteUDP = new UdpClient((int) puerto);
            try
            {
                while (true)
                {
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = ClienteUDP.Receive(ref anyIP);
                    if (data != null && data.Length > 0 )
                    {
                        EventoEnJuego eventoEnJuego = Deserializar(data);
                        EventoRecibido?.Invoke(eventoEnJuego);   
                    }
                }
            }
            catch (Exception err)
            {
                Debug.Write(err.Message);
            }
        }

        /// <summary>
        /// Libera los recursos utilizados al escuchar paquetes en la red
        /// </summary>
        public void LiberarRecursos()
        {
            if(ClienteUDP != null)
            {
                ClienteUDP.Dispose();
            }
            
        }
    }
}