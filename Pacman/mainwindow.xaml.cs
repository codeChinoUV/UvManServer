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
        private const String ENDPOINT_TCP_SERVICIO_CUENTA = "net.tcp://localhost:8092/CuentaService";
        private const String ENDPOINT_TCP_SERVICIO_SESION = "net.tcp://localhost:7972/SessionService";
        private const String ENDPOINT_TCP_SERVICIO_JUEGO = "net.tcp://localhost:8292/GameService";

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

        /// <summary>
        /// Establece el Item source de la tabla DGUsuariosConectados
        /// </summary>
        private void CargarUsuariosConectadosEnLaTabla()
        {
            DGUsuariosConectados.ItemsSource = null;
            DGUsuariosConectados.ItemsSource = cuentasConectadas;
        }

        /// <summary>
        /// Establece el Item source de la tabla DGSalasConectadas
        /// </summary>
        private void CargarInformacionSalasCreadasEnLaTabla()
        {
            DGSalasConectadas.ItemsSource = null;
            DGSalasConectadas.ItemsSource = SalasActuales;
        }

        /// <summary>
        /// Obtiene la direccion ip del adaptador de red
        /// </summary>
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

        /// <summary>
        /// Muestra las direccines TCP de los servicios
        /// </summary>
        private void MostrarDireccionesDeServicios()
        {
            lDireccionIpServicioChat.Text = ENDPOINT_TCP_SERVICIO_CHAT;
            lDireccionIpServicioCuenta.Text = ENDPOINT_TCP_SERVICIO_CUENTA;
            lDireccionIpServicioSesion.Text = ENDPOINT_TCP_SERVICIO_SESION;
            lDireccionIpServicioDeJuego.Text = ENDPOINT_TCP_SERVICIO_JUEGO;
            lDireccionIp.Content = DireccionIP;
        }

        /// <summary>
        /// Se suscribe a los serviciosde el manejador de sesiones
        /// </summary>
        private void SuscribirseAEscuchaDeServicioDeSession()
        {
            ManejadorDesesiones.UsuarioConectado += NuevoUsuarioEnSession;
            ManejadorDesesiones.UsuarioDesconectado += UsuarioDejoSession;
        }

        /// <summary>
        /// Se suscribe a los servicios de el manejador de salas
        /// </summary>
        private void SuscribirseAEscuchaDeServicioDeSala()
        {
            ManejadorDeSalas.SalaCreada += SeCreoUnaNuevaSala;
            ManejadorDeSalas.SalaDestriuda += SeDestruyoUnaSala;
            ManejadorDeSalas.SeUnioASala += SeUnioCuentaASala;
            ManejadorDeSalas.DejoSala += DejoCuentaSala;
        }

        /// <summary>
        /// Inicializa todos los elementos de la pantalla y se suscribe a los servicios
        /// de manejo de sesiones y manejo de salas
        /// </summary>
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

        /// <summary>
        /// Inicia el host del servicio de cuentas
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
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

        /// <summary>
        /// Inicia el host del servicio de chat
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
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

        /// <summary>
        /// Crea el host del servicio de sesion
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventoArgs</param>
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

        /// <summary>
        /// Crea un hilo para la esucha de paquetes udp
        /// </summary>
        private void InicializarEscuchaDePaquetesUDP()
        {
            HiloDeEscuchaPaquetesUDP = new Thread(RecibidorPaquetesUDP1.RecibirDatos);
            HiloDeEscuchaPaquetesUDP2 = new Thread(RecibidorPaquetesUDP2.RecibirDatos);
            HiloDeEscuchaPaquetesUDP.Start(PUERTO_ESCUCHA_UDP_1);
            HiloDeEscuchaPaquetesUDP2.Start(PUERTO_ESCUCHA_UDP_2);
        }

        /// <summary>
        /// Se suscribe a los eventos de escucha de paquetes udp
        /// </summary>
        private void SuscribirseAEventosDeEscuchaDePaquetesUDP()
        {
            RecibidorPaquetesUDP1.EventoRecibido += ManejadorDeSalas.ReplicarDatosRecibidosASala;
            RecibidorPaquetesUDP2.EventoRecibido += ManejadorDeSalas.ReplicarDatosRecibidosASala;
        }

        /// <summary>
        /// Inicia el host del servio de juego
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">RoutedEventArgs</param>
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

        /// <summary>
        /// Cierra el host de servicio de chat
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void BDetenerServicioChat_Click(object sender, RoutedEventArgs e)
        {
            DetenerServicioChat();
        }

        /// <summary>
        /// Detiene el host del servicio de cuenta
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
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

        /// <summary>
        /// Detiene el host del servicio de sesion
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
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

        /// <summary>
        /// Detiene el host del servicio del juego
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="args">RoutedEventArgs</param>
        private void BDetenerServicioDelJuego_Click(object sender, RoutedEventArgs args)
        {
            DetenerServicioJuego();
        }
        
        /// <summary>
        /// Actualiza la tabla de usuarios conectados de la interfaz
        /// </summary>
        /// <param name="cuenta"></param>
        private void UsuarioDejoSession(CuentaModel cuenta)
        {
            cuentasConectadas.Remove(cuenta);
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(CargarUsuariosConectadosEnLaTabla));
            }
        }
   
        /// <summary>
        /// Agrega un nuevo usuario a la lista de usuarios conectados
        /// </summary>
        /// <param name="cuenta">CuentaModel</param>
        private void NuevoUsuarioEnSession(CuentaModel cuenta)
        {
            cuentasConectadas.Add(cuenta);
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(CargarUsuariosConectadosEnLaTabla));
            }
        }

        /// <summary>
        /// Cambia el etado del host en el label de estadoServicioCuenta
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void HostCuentaOnClosed(Object sender, EventArgs e)
        {
            lEstadoServicioCuenta.Content = "Servicio cerrado";
        }

        /// <summary>
        /// Cambia el estado del host en el label de estadoServicioChat
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        private void HostChatOnClosed(Object sender, EventArgs e)
        {
            lEstadoServicioChat.Content = "Servicio cerrado";
        }

        /// <summary>
        /// Cambia el estado del host en el label de estadoServicioSesion y detiene el host del juego y del chat
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void HostSesionOnClosed(Object sender, EventArgs e)
        {
            lEstadoServicioSesion.Content = "Servicio cerrado";
            DetenerServicioJuego();
            DetenerServicioChat();
        }

        /// <summary>
        /// Agrega a una lista de salas la nueva sala creada
        /// </summary>
        /// <param name="NuevaSala">Sala</param>
        private void SeCreoUnaNuevaSala(Sala NuevaSala)
        {
            SalasActuales.Add(NuevaSala);
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(CargarInformacionSalasCreadasEnLaTabla));
            }
        }
   
        /// <summary>
        /// Busca el id de la sala destruida, la elimina de la lista de
        /// la salas creadas y actualiza la tabla de las salas creadas en la interfaz
        /// </summary>
        /// <param name="SalaDestruida">Sala</param>
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

        /// <summary>
        /// Actualiza la informacion de la sala en la interfaz grafica
        /// </summary>
        /// <param name="id">String</param>
        private void SeUnioCuentaASala(String id)
        {
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(CargarInformacionSalasCreadasEnLaTabla));
            }
        }

        /// <summary>
        /// Actualiza la infromacion de la sala en la interfaz grafica
        /// </summary>
        /// <param name="id">String</param>
        private void DejoCuentaSala(String id)
        {
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(new ThreadStart(CargarInformacionSalasCreadasEnLaTabla));
            }
        }

        /// <summary>
        /// Libera los recursos utilizados por el hilo de escucha UDP y termina los hilos
        /// </summary>
        private void CerrarEscuchadorDePaquetesUDP()
        {
            RecibidorPaquetesUDP1.LiberarRecursos();
            HiloDeEscuchaPaquetesUDP?.Abort();
            RecibidorPaquetesUDP2.LiberarRecursos();
            HiloDeEscuchaPaquetesUDP2.Abort();
        }

        /// <summary>
        /// Libera los recursos utilizados antes de cerrar la ventana
        /// </summary>
        /// <param name="Sender">object</param>
        /// <param name="e">EventArgs</param>
        void OnClosing(object Sender, EventArgs e)
        {
            RecibidorPaquetesUDP1.LiberarRecursos();
            RecibidorPaquetesUDP2.LiberarRecursos();
            HiloDeEscuchaPaquetesUDP?.Abort();
            HiloDeEscuchaPaquetesUDP2?.Abort();
            ManejadorDesesiones.TerminarTodosLosHilosDeEscucha();
        }

        /// <summary>
        /// Detiene el host del servicio de chat
        /// </summary>
        private void DetenerServicioChat()
        {
            if (ChatHost != null)
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

        /// <summary>
        /// Detiene el host del servicio del juego
        /// </summary>
        private void DetenerServicioJuego()
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

    }
}