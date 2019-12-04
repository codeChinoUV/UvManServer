using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameService.Dominio
{
    public class UdpSender
    {
        private IPEndPoint IpEnviarPaquete1;
        private IPEndPoint IpEnviarPaquete2;
        private UdpClient ClienteUDP;

        public UdpSender(String direccionIp, int puerto, int puerto2)
        {
            if (direccionIp == "::1")
            {
                direccionIp = "127.0.0.1";
            }
            IpEnviarPaquete1 = new IPEndPoint(IPAddress.Parse(direccionIp), puerto);
            IpEnviarPaquete2 = new IPEndPoint(IPAddress.Parse(direccionIp), puerto2);
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
                ClienteUDP.Send(datos, datos.Length, IpEnviarPaquete1);
                ClienteUDP.Send(datos, datos.Length, IpEnviarPaquete2);
            }
        }
    }
}