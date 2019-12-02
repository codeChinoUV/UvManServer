using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;


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
            using (MemoryStream StreamDeMemoria = new MemoryStream(byteArray))
            {
                DataContractSerializer Serializador = new DataContractSerializer(typeof(EventoEnJuego));
                EventoEnJuego movimientoDelJugador = (EventoEnJuego) Serializador.ReadObject(StreamDeMemoria);
                return movimientoDelJugador;
            }
        }

        public void RecibirDatos()
        {
            ClienteUDP = new UdpClient(8090);
            try
            {
                while (true)
                {
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = ClienteUDP.Receive(ref anyIP);
                    Debug.WriteLine("Estoy escuchando en el puerto 0");
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
            ClienteUDP.Dispose();
        }
    }
}