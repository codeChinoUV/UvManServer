using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameService.Dominio
{
    public class UdpSender
    {
        private readonly int PUERTO = 8296;
        private IPEndPoint IpEnviarPaquete;
        private UdpClient ClienteUDP;

        public UdpSender(String direccionIp)
        {
            if (direccionIp == "::1")
            {
                direccionIp = "127.0.0.1";
            }
            IpEnviarPaquete = new IPEndPoint(IPAddress.Parse(direccionIp), PUERTO);
        }
        
        private static byte[] SerializarAArregloDeBytes(EventoEnJuego eventoEnJuego)
        {
            if (eventoEnJuego != null)
            {
                using (MemoryStream StreamDeMemoria = new MemoryStream())
                {
                    BinaryFormatter FormateadorBinario = new BinaryFormatter();
                    FormateadorBinario.Serialize(StreamDeMemoria, eventoEnJuego);
                    return StreamDeMemoria.ToArray();
                }
                
            }
            
            return null;
        }

        public void EnviarPaquete(EventoEnJuego eventoEnJuego)
        {
            ClienteUDP = new UdpClient();
            if (eventoEnJuego != null)
            {
                byte[] datos = SerializarAArregloDeBytes(eventoEnJuego);
                ClienteUDP.Send(datos, datos.Length, IpEnviarPaquete);
            }
        }
    }
}