using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows;
using GameChatService;
using GameChatService.Servicio;
using CuentaService.Contrato;
using SessionService.Contrato;
using System.Net;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.Util;
using System.Threading;

namespace Pacman
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const String ENDPOINT_TCP_SERVICIO_CHAT = "net.tcp://localhost:8192/ChatService";
        private const String ENDPOINT_HTTP_SERVICIO_CHAT = "http://localhost:8182/ChatService";
        private const String ENDPOINT_TCP_SERVICIO_CUENTA = "net.tcp://localhost:8092/CuentaService";
        private const String ENDPOINT_HTTP_SERVICIO_CUENTA = "http://localhost:8082/CuentaService";
        private const String ENDPOINT_TCP_SERVICIO_SESION = "net.tcp://localhost:7972/SessionService";
        private const String ENDPOINT_HTTP_SERVICIO_SESION = "http://localhost:7982/SessionService";

        private ServiceHost CuentaHost;
        private ServiceHost ChatHost;
        private ServiceHost SesionHost;
        private String DireccionIP;
        private SessionManager ManejadorDesesiones = SessionManager.GetSessionManager();

        public List<CuentaModel> cuentasConectadas = new List<CuentaModel>();
        

        private void CargarUsuariosConectados()
        {
            DGUsuariosConectados.ItemsSource = null;
            DGUsuariosConectados.ItemsSource = cuentasConectadas;
        }

        private void ObtenerDireccionIpLocal()
        {
            IPHostEntry Host;
            Host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress Ip in Host.AddressList)
            {
                if (Ip.AddressFamily.ToString() == "InterNetwork")
                {
                    DireccionIP = Ip.ToString();
                }
            }
        }

        private void MostrarDireccionesDeServicios()
        {
            lDireccionIpServicioChat.Text = ENDPOINT_TCP_SERVICIO_CHAT;
            lDireccionIpServicioCuenta.Text = ENDPOINT_TCP_SERVICIO_CUENTA;
            lDireccionIpServicioSesion.Text = ENDPOINT_TCP_SERVICIO_SESION;
            lDireccionIp.Content = DireccionIP;
        }

        private void SuscribirseAEscuchaDeServicioDeSession()
        {
            ManejadorDesesiones.UsuarioConectado += NuevoUsuarioEnSession;
            ManejadorDesesiones.UsuarioDesconectado += UsuarioDejoSession;
        }

        public MainWindow()
        {
            InitializeComponent();
            ObtenerDireccionIpLocal();
            CargarUsuariosConectados();
            MostrarDireccionesDeServicios();
            SuscribirseAEscuchaDeServicioDeSession();
        }

        private void BIniciarServicioCuenta_Click(object sender, RoutedEventArgs e)
        {
            bIniciarServicioCuenta.IsEnabled = false;
            CuentaHost = new ServiceHost(typeof(CuentaService.Servicio.CuentaService));
            try
            {
                CuentaHost.Closed += hostCuentaOnClosed;
                CuentaHost.Open();
            }
            catch (Exception excepcion)
            {
                lEstadoServicioCuenta.Content = excepcion.Message;
            }
            finally
            {
                if (CuentaHost.State == CommunicationState.Opened)
                {
                    lEstadoServicioCuenta.Content = "Activo";
                    bDetenerServicioCuenta.IsEnabled = true;
                }
            }
        }

        private void BIniciarServicioChat_Click(object sender, RoutedEventArgs e)
        {
            ChatHost = new ServiceHost(typeof(ChatService));
            bIniciarServicioChat.IsEnabled = false;
            try
            {
                ChatHost.Closed += hostChatOnClosed;
                ChatHost.Open();
            }
            catch (Exception excepcionDelServicio)
            {
                lEstadoServicioChat.Content = excepcionDelServicio.Message;
            }
            finally
            {
                if (ChatHost.State == CommunicationState.Opened)
                {
                    lEstadoServicioChat.Content = "Activo";
                    bDetenerServicioChat.IsEnabled = true;
                }
            }
        }

        private void BIniciarServicioSesion_Click(object sender, RoutedEventArgs e)
        {
            bIniciarServicioSesion.IsEnabled = false;
            SesionHost = new ServiceHost(typeof(SessionService.Servicio.SessionService));
            try
            {
                SesionHost.Closed += hostSesionOnClosed;
                SesionHost.Open();
            }
            catch (Exception excepcionDelServicio)
            {
                lEstadoServicioSesion.Content = excepcionDelServicio.Message;
            }
            finally
            {
                if (SesionHost.State == CommunicationState.Opened)
                {
                    lEstadoServicioSesion.Content = "Activo";
                    bDetenerServicioSesion.IsEnabled = true;
                }
            }
        }

        private void BDetenerServicioChat_Click(object sender, RoutedEventArgs e)
        {
            if (ChatHost != null )
            {
                try
                {
                    ChatHost.Close();
                }
                catch (Exception excepcion)
                {
                    lEstadoServicioChat.Content = excepcion.Message;
                }
                finally
                {
                    if (ChatHost.State == CommunicationState.Closed)
                    {
                        lEstadoServicioChat.Content = "Cerrada";
                        bIniciarServicioChat.IsEnabled = true;
                        bDetenerServicioChat.IsEnabled = false;
                    }
                }
            }
        }

        

        private void BDetenerServicioCuenta_Click(object sender, RoutedEventArgs e)
        {
            if (CuentaHost != null)
            {
                try
                {
                    CuentaHost.Close();
                }
                catch (Exception excepcion)
                {
                    lEstadoServicioCuenta.Content = excepcion.Message;
                }
                finally
                {
                    if (CuentaHost.State == CommunicationState.Closed)
                    {
                        lEstadoServicioCuenta.Content = "Cerrada";
                        bIniciarServicioCuenta.IsEnabled = true;
                        bDetenerServicioCuenta.IsEnabled = false;
                    }
                }
            }
        }

        private void BDetenerServicioSesion_Click(object sender, RoutedEventArgs e)
        {
            if (SesionHost != null)
            {
                try
                {
                    SesionHost.Close();
                }
                catch (Exception excepcion)
                {
                    lEstadoServicioSesion.Content = excepcion.Message;
                }
                finally
                {
                    if (SesionHost.State == CommunicationState.Closed)
                    {
                        lEstadoServicioSesion.Content = "Cerrada";
                        bIniciarServicioSesion.IsEnabled = true;
                        bDetenerServicioSesion.IsEnabled = false;
                    }
                }
            }
        }
        
        private void UsuarioDejoSession(CuentaModel cuenta)
        {
            cuentasConectadas.Remove(cuenta);
            Dispatcher.BeginInvoke(new ThreadStart(CargarUsuariosConectados));
        }
   
        private void NuevoUsuarioEnSession(CuentaModel cuenta)
        {
            cuentasConectadas.Add(cuenta);
            Dispatcher.BeginInvoke(new ThreadStart(CargarUsuariosConectados));
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

    }
}
