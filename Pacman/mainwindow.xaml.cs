using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using GameChatService.Servicio;
using System.Net;
using LogicaDelNegocio.Modelo;
using LogicaDelNegocio.Util;
using System.Threading;
using GameService.Dominio;
using System.ServiceModel.Channels;

namespace Pacman
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int PUERTO_ESCUCHA_UDP_1 = 8090;
        private readonly int PUERTO_ESCUCHA_UDP_2 = 8091;
        
        private const String ENDPOINT_TCP_SERVICIO_CHAT = "net.tcp://localhost:8192/ChatService";
        private const String ENDPOINT_HTTP_SERVICIO_CHAT = "http://localhost:8182/ChatService";
        private const String ENDPOINT_TCP_SERVICIO_CUENTA = "net.tcp://localhost:8092/CuentaService";
        private const String ENDPOINT_HTTP_SERVICIO_CUENTA = "http://localhost:8082/CuentaService";
        private const String ENDPOINT_TCP_SERVICIO_SESION = "net.tcp://localhost:7972/SessionService";
        private const String ENDPOINT_HTTP_SERVICIO_SESION = "http://localhost:7982/SessionService";

        private ServiceHost CuentaHost;
        private ServiceHost ChatHost;
        private ServiceHost SesionHost;
        private ServiceHost JuegoHost;
        private String DireccionIP;
        private SessionManager ManejadorDesesiones = SessionManager.GetSessionManager();
        private SalaManager ManejadorDeSalas = SalaManager.GetSalaManager();
        private UdpReciver RecibidorPaquetesUDP1 = new UdpReciver();
        public UdpReciver RecibidorPaquetesUDP2 = new UdpReciver();

        private Thread HiloDeEscuchaPaquetesUDP;
        private Thread HiloDeEscuchaPaquetesUDP2;

        public List<CuentaModel> cuentasConectadas = new List<CuentaModel>();
        public List<Sala> SalasActuales = new List<Sala>();

        private void CargarUsuariosConectadosEnLaTabla()
        {
            DGUsuariosConectados.ItemsSource = null;
            DGUsuariosConectados.ItemsSource = cuentasConectadas;
        }

        private void CargarInformacionSalasCreadasEnLaTabla()
        {
            DGSalasConectadas.ItemsSource = null;
            DGSalasConectadas.ItemsSource = SalasActuales;
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

        private void SuscribirseAEscuchaDeServicioDeSala()
        {
            ManejadorDeSalas.SalaCreada += SeCreoUnaNuevaSala;
            ManejadorDeSalas.SalaDestriuda += SeDestruyoUnaSala;
            ManejadorDeSalas.SeUnioASala += SeUnioCuentaASala;
            ManejadorDeSalas.DejoSala += DejoCuentaSala;
        }

        public MainWindow()
        {
            InitializeComponent();
            ObtenerDireccionIpLocal();
            CargarUsuariosConectadosEnLaTabla();
            CargarInformacionSalasCreadasEnLaTabla();
            MostrarDireccionesDeServicios();
            SuscribirseAEscuchaDeServicioDeSession();
            SuscribirseAEscuchaDeServicioDeSala();
        }

        private void BIniciarServicioCuenta_Click(object sender, RoutedEventArgs e)
        {
            bIniciarServicioCuenta.IsEnabled = false;
            CuentaHost = new ServiceHost(typeof(CuentaService.Servicio.CuentaService));
            try
            {
                CuentaHost.Closed += HostCuentaOnClosed;
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
                ChatHost.Closed += HostChatOnClosed;
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
                SesionHost.Closed += HostSesionOnClosed;
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

        private void InicializarEscuchaDePaquetesUDP()
        {
            HiloDeEscuchaPaquetesUDP = new Thread(RecibidorPaquetesUDP1.RecibirDatos);
            HiloDeEscuchaPaquetesUDP2 = new Thread(RecibidorPaquetesUDP2.RecibirDatos);
            HiloDeEscuchaPaquetesUDP.Start(PUERTO_ESCUCHA_UDP_1);
            HiloDeEscuchaPaquetesUDP2.Start(PUERTO_ESCUCHA_UDP_2);
        }

        private void SuscribirseAEventosDeEscuchaDePaquetesUDP()
        {
            RecibidorPaquetesUDP1.EventoRecibido += ManejadorDeSalas.ReplicarDatosRecibidosASala;
            RecibidorPaquetesUDP2.EventoRecibido += ManejadorDeSalas.ReplicarDatosRecibidosASala;
        }

        private void BIniciarServicioDelJuego_Click(object sender, RoutedEventArgs e)
        {
            bIniciarServicioDelJuego.IsEnabled = false;
            JuegoHost = new ServiceHost(typeof(GameService.Servicio.GameService));
            try
            {
                JuegoHost.Closed += HostSesionOnClosed;
                JuegoHost.Open();
                SuscribirseAEventosDeEscuchaDePaquetesUDP();
                InicializarEscuchaDePaquetesUDP();
            }
            catch (Exception excepcionDelServicio)
            {
                lEstadoServicioDeJuego.Content = excepcionDelServicio.Message;
            }
            finally
            {
                if (JuegoHost.State == CommunicationState.Opened)
                {
                    lEstadoServicioDeJuego.Content = "Activo";
                    bDetenerServicioDelJuego.IsEnabled = true;
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
                    ManejadorDesesiones.TerminarTodosLosHilosDeEscucha();
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

        private void BDetenerServicioDelJuego_Click(object sender, RoutedEventArgs args)
        {
            if (JuegoHost != null)
            {
                try
                {
                    JuegoHost.Close();
                    CerrarEscuchadorDePaquetesUDP();
                }
                catch (Exception ex)
                {
                    lEstadoServicioDeJuego.Content = ex.Message;
                }
                finally
                {
                    if (JuegoHost.State == CommunicationState.Closed)
                    {
                        lEstadoServicioDeJuego.Content = "Cerrada";
                        bIniciarServicioDelJuego.IsEnabled = true;
                        bDetenerServicioDelJuego.IsEnabled = false;
                    }
                }
            }
        }
        
        private void UsuarioDejoSession(CuentaModel cuenta)
        {
            cuentasConectadas.Remove(cuenta);
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(CargarUsuariosConectadosEnLaTabla));
            }
        }
   
        private void NuevoUsuarioEnSession(CuentaModel cuenta)
        {
            cuentasConectadas.Add(cuenta);
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(CargarUsuariosConectadosEnLaTabla));
            }
        }

        private void HostCuentaOnClosed(Object sender, EventArgs e)
        {
            lEstadoServicioCuenta.Content += "Servicio cerrado";
        }

        private void HostChatOnClosed(Object sender, EventArgs e)
        {
            lEstadoServicioChat.Content += "Servicio cerrado";
        }

        private void HostSesionOnClosed(Object sender, EventArgs e)
        {
            lEstadoServicioSesion.Content += "Servicio cerrado";
        }

        private void SeCreoUnaNuevaSala(Sala NuevaSala)
        {
            SalasActuales.Add(NuevaSala);
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(CargarInformacionSalasCreadasEnLaTabla));
            }
        }
   
        private void SeDestruyoUnaSala(Sala SalaDestruida)
        {
            Sala SalaADestruir = null;
            foreach(Sala SalaEnTabla in SalasActuales)
            {
                if(SalaEnTabla.Id == SalaDestruida.Id)
                {
                    SalaADestruir = SalaEnTabla;
                }
            }
            if(SalaADestruir != null)
            {
                SalasActuales.Remove(SalaDestruida);
                if (Dispatcher != null)
                {
                    Dispatcher.BeginInvoke(new ThreadStart(CargarInformacionSalasCreadasEnLaTabla));
                }
            }
        }

        private void SeUnioCuentaASala(String id)
        {
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(CargarInformacionSalasCreadasEnLaTabla));
            }
        }

        private void DejoCuentaSala(String id)
        {
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(CargarInformacionSalasCreadasEnLaTabla));
            }
        }

        private void CerrarEscuchadorDePaquetesUDP()
        {
            RecibidorPaquetesUDP1.LiberarRecursos();
            HiloDeEscuchaPaquetesUDP?.Abort();
            RecibidorPaquetesUDP2.LiberarRecursos();
            HiloDeEscuchaPaquetesUDP2.Abort();
        }


        void OnClosing(object Sender, EventArgs e)
        {
            RecibidorPaquetesUDP1.LiberarRecursos();
            RecibidorPaquetesUDP2.LiberarRecursos();
            HiloDeEscuchaPaquetesUDP?.Abort();
            HiloDeEscuchaPaquetesUDP2?.Abort();
            ManejadorDesesiones.TerminarTodosLosHilosDeEscucha();
        }

    }
}