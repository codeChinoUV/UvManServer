using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows;
using GameChatService;
using GameChatService.Servicio;
using CuentaService.Contrato;
using CuentaService.Servicio;
using SessionService.Contrato;
using System.Windows.Controls;

namespace Pacman
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceHost cuentaHost;
        private ServiceHost chatHost;
        private ServiceHost sesionHost;
        private String ENDPOINT_TCP_SERVICIO_CHAT = "net.tcp://localhost:8192/ChatService";
        private String ENDPOINT_HTTP_SERVICIO_CHAT = "http://localhost:8182/ChatService";
        private String ENDPOINT_TCP_SERVICIO_CUENTA = "net.tcp://localhost:8092/CuentaService";
        private String ENDPOINT_HTTP_SERVICIO_CUENTA = "http://localhost:8082/CuentaService";
        private String ENDPOINT_TCP_SERVICIO_SESION = "net.tcp://localhost:7972/SessionService";
        private String ENDPOINT_HTTP_SERVICIO_SESION = "http://localhost:7982/SessionService";


        public MainWindow()
        {
            InitializeComponent();
            lDireccionIpServicioChat.Text = ENDPOINT_TCP_SERVICIO_CHAT;
            lDireccionIpServicioCuenta.Text = ENDPOINT_TCP_SERVICIO_CUENTA;
            lDireccionIpServicioSesion.Text = ENDPOINT_TCP_SERVICIO_SESION;
        }



        private void BIniciarServicioCuenta_Click(object sender, RoutedEventArgs e)
        {
            bIniciarServicioCuenta.IsEnabled = false;
            Uri[] uris = new Uri[2];
            uris[0] = new Uri(ENDPOINT_TCP_SERVICIO_CUENTA);
            uris[1] = new Uri(ENDPOINT_HTTP_SERVICIO_CUENTA);
            ICuentaService servicioDeCuenta = new CuentaService.Servicio.CuentaService();
            cuentaHost = new ServiceHost(servicioDeCuenta, uris);
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            cuentaHost.AddServiceEndpoint(typeof(ICuentaService), binding, String.Empty);
            ServiceMetadataBehavior comportamientoDelaMetadata = new ServiceMetadataBehavior();
            comportamientoDelaMetadata.HttpGetEnabled = true;
            cuentaHost.Description.Behaviors.Add(comportamientoDelaMetadata);
            try
            {
                cuentaHost.Closed += hostCuentaOnClosed;
                cuentaHost.Open();
            }
            catch (Exception excepcion)
            {
                lEstadoServicioCuenta.Content = excepcion.Message;
            }
            finally
            {
                if (cuentaHost.State == CommunicationState.Opened)
                {
                    lEstadoServicioCuenta.Content = "Activo";
                    bDetenerServicioCuenta.IsEnabled = true;
                }
            }
        }

        private void hostCuentaOnClosed(Object sender, EventArgs e)
        {
            lEstadoServicioCuenta.Content += "Servicio cerrado";
        }

        private void hostChatOnClosed(Object sender, EventArgs e)
        {
            lEstadoServicioChat.Content += "Servicio cerrado";
        }

        private void hostSesionOnClosed(Object sender, EventArgs e)
        {
            lEstadoServicioSesion.Content += "Servicio cerrado";
        }

        private void BDetenerServicioChat_Click(object sender, RoutedEventArgs e)
        {
            if (chatHost != null )
            {
                try
                {
                    chatHost.Close();
                }
                catch (Exception excepcion)
                {
                    lEstadoServicioChat.Content = excepcion.Message;
                }
                finally
                {
                    if (chatHost.State == CommunicationState.Closed)
                    {
                        lEstadoServicioChat.Content = "Cerrada";
                        bIniciarServicioChat.IsEnabled = true;
                        bDetenerServicioChat.IsEnabled = false;
                    }
                }
            }
        }

        private void BIniciarServicioChat_Click(object sender, RoutedEventArgs e)
        {
            bIniciarServicioChat.IsEnabled = false;
            Uri[] uris = new Uri[2];
            uris[0] = new Uri(ENDPOINT_TCP_SERVICIO_CHAT);
            uris[1] = new Uri(ENDPOINT_HTTP_SERVICIO_CHAT);
            IChatService servicioDeChat = new ChatService();
            chatHost = new ServiceHost(servicioDeChat, uris);
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            chatHost.AddServiceEndpoint(typeof(IChatService), binding, String.Empty);
            ServiceMetadataBehavior comportamientoDelaMetadata = new ServiceMetadataBehavior();
            comportamientoDelaMetadata.HttpGetEnabled = true;
            chatHost.Description.Behaviors.Add(comportamientoDelaMetadata);
            try
            {
                chatHost.Closed += hostChatOnClosed;
                chatHost.Open();
            }
            catch (Exception excepcionDelServicio)
            {
                lEstadoServicioChat.Content = excepcionDelServicio.Message;
            }
            finally
            {
                if (chatHost.State == CommunicationState.Opened)
                {
                    lEstadoServicioChat.Content = "Activo";
                    bDetenerServicioChat.IsEnabled = true;
                }
            }
        }

        private void BDetenerServicioCuenta_Click(object sender, RoutedEventArgs e)
        {
            if (cuentaHost != null)
            {
                try
                {
                    cuentaHost.Close();
                }
                catch (Exception excepcion)
                {
                    lEstadoServicioCuenta.Content = excepcion.Message;
                }
                finally
                {
                    if (cuentaHost.State == CommunicationState.Closed)
                    {
                        lEstadoServicioCuenta.Content = "Cerrada";
                        bIniciarServicioCuenta.IsEnabled = true;
                        bDetenerServicioCuenta.IsEnabled = false;
                    }
                }
            }
        }

        private void BIniciarServicioSesion_Click(object sender, RoutedEventArgs e)
        {
            bIniciarServicioSesion.IsEnabled = false;
            Uri[] uris = new Uri[2];
            uris[0] = new Uri(ENDPOINT_TCP_SERVICIO_SESION);
            uris[1] = new Uri(ENDPOINT_HTTP_SERVICIO_SESION);
            ISessionService servicioDeSesion = new SessionService.Servicio.SessionService();
            sesionHost = new ServiceHost(servicioDeSesion, uris);
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            sesionHost.AddServiceEndpoint(typeof(ISessionService), binding, String.Empty);
            ServiceMetadataBehavior comportamientoDelaMetadata = new ServiceMetadataBehavior();
            comportamientoDelaMetadata.HttpGetEnabled = true;
            sesionHost.Description.Behaviors.Add(comportamientoDelaMetadata);
            try
            {
                sesionHost.Closed += hostSesionOnClosed;
                sesionHost.Open();
            }
            catch (Exception excepcionDelServicio)
            {
                throw;
                //lEstadoServicioSesion.Content = excepcionDelServicio.Message;
            }
            finally
            {
                if (sesionHost.State == CommunicationState.Opened)
                {
                    lEstadoServicioSesion.Content = "Activo";
                    bDetenerServicioSesion.IsEnabled = true;
                }
            }
        }

        private void BDetenerServicioSesion_Click(object sender, RoutedEventArgs e)
        {
            if (sesionHost != null)
            {
                try
                {
                    sesionHost.Close();
                }
                catch (Exception excepcion)
                {
                    lEstadoServicioSesion.Content = excepcion.Message;
                }
                finally
                {
                    if (sesionHost.State == CommunicationState.Closed)
                    {
                        lEstadoServicioSesion.Content = "Cerrada";
                        bIniciarServicioSesion.IsEnabled = true;
                        bDetenerServicioSesion.IsEnabled = false;
                    }
                }
            }
        }
    }
}
