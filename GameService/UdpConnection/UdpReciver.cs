using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameService.Dominio
{
    public class UdpReciver
    {
        private UdpClient ClienteUDP;
        
        public delegate void RecibirEventoEnJuego(EventoEnJuego eventoEnJuego);
        public event RecibirEventoEnJuego EventoRecibido;

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

        public void LiberarRecursos()
        {
            if(ClienteUDP != null)
            {
                ClienteUDP.Dispose();
            }
            
        }
    }
}