using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameService.Dominio
{
    /// <summary>
    /// Se encarga de enviar paquetes UDP a dos diferentes puertos
    /// </summary>
    public class UdpSender
    {
        private IPEndPoint IpEnviarPaquete1;
        private IPEndPoint IpEnviarPaquete2;
        private UdpClient ClienteUDP;

        /// <summary>
        /// Constructor con los puertos a donde se enviaran los paquetes UDP
        /// </summary>
        /// <param name="direccionIp">String</param>
        /// <param name="puerto">int</param>
        /// <param name="puerto2">int</param>
        public UdpSender(String direccionIp, int puerto, int puerto2)
        {
            if (direccionIp == "::1")
            {
                direccionIp = "127.0.0.1";
            }
            IpEnviarPaquete1 = new IPEndPoint(IPAddress.Parse(direccionIp), puerto);
            IpEnviarPaquete2 = new IPEndPoint(IPAddress.Parse(direccionIp), puerto2);
        }
        
        /// <summary>
        /// Serializa un EventoEnJuego en un arreglo de bytes
        /// </summary>
        /// <param name="eventoEnJuego">EventoEnJuego</param>
        /// <returns>Arreglo de bytes del EventoEnJuegoSerializado</returns>
        private static byte[] SerializarAArregloDeBytes(EventoEnJuego eventoEnJuego)
        {
            if (eventoEnJuego != null)
            {
                BinaryFormatter FormateadorBinario = new BinaryFormatter();
                using (MemoryStream StreamDeMemoria = new MemoryStream())
                {
                    FormateadorBinario.Serialize(StreamDeMemoria, eventoEnJuego);
                    return StreamDeMemoria.ToArray();
                }
                
            }
            
            return null;
        }

        /// <summary>
        /// Envia un paquete Udp a los dos puertos y de la direccion ip
        /// </summary>
        /// <param name="eventoEnJuego">EventoEnJuego</param>
        public void EnviarPaquete(EventoEnJuego eventoEnJuego)
        {
            ClienteUDP = new UdpClient();
            if (eventoEnJuego != null)
            {
                byte[] datos = SerializarAArregloDeBytes(eventoEnJuego);
                ClienteUDP.Send(datos, datos.Length, IpEnviarPaquete1);
                ClienteUDP.Send(datos, datos.Length, IpEnviarPaquete2);
            }
        }
    }
}